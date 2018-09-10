using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlingController : MonoBehaviour {

	#region character components
	Animator animator;
	CharacterController controller;
	Rigidbody rigidbody;
	Transform mainCameraT;
	#endregion
	Vector3 targetDirection;
	float hitboxHeight = 0.5f;
	float hitboxRadius = 0.12f;
	float targetRotation;
	float currentSpeed;
	float targetSpeed;
	float speedSmoothVelocity;
	float turnSmoothTime = 5f;
	static float crawlSpeedSmoothTime = 0.1f;
	static float crawlSpeed = 2f;
	public GameObject specialMovementTrigger;

	// Use this for initialization
	void Init(){
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody>();
		mainCameraT = Camera.main.transform;
		targetDirection = new Vector3(0f, 0f, 0f);
		SetCrawlHitbox();
	}

	void OnEnable(){
		Init();
	}

	void Start () {
		Init();
	}

	// Update is called once per frame
	void Update () {
		var inputX =  Input.GetAxisRaw("Horizontal");
		var inputZ = Input.GetAxisRaw("Vertical");
	
		targetDirection = new Vector3(inputX, 0f, inputZ);
		targetRotation = mainCameraT.eulerAngles.y;

		Quaternion target = Quaternion.Euler(Vector3.up * targetRotation);
		//transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * turnSmoothTime);
		transform.rotation = target;

		var targetSpeed = crawlSpeed * targetDirection.magnitude;
		var smoothTime = crawlSpeedSmoothTime;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
		Vector3 vel = targetDirection * currentSpeed;

		controller.Move(vel * Time.deltaTime);

		animator.SetBool("IsCrawling", true);
		animator.SetFloat("HSpeed", currentSpeed);
	}

	bool CheckIfCrawlSpace(){
		var inCrawlSpace = false;
		RaycastHit hit;
		var layers = 1 << 9; 
		layers = ~layers; // ignore player

		// check if enough space all around character to stand
		// inCrawlSpace = Physics.Raycast(transform.position, Vector3.forward , out hit, .85f)
		// 		& Physics.Raycast(transform.position, Vector3.back , out hit, .85f)
		// 		& Physics.Raycast(transform.position, Vector3.left , out hit, .85f)
		// 		& Physics.Raycast(transform.position, Vector3.right , out hit, .85f)
		// 		& Physics.Raycast(transform.position, Vector3.up , out hit, .85f);

		inCrawlSpace = Physics.Raycast(transform.position, Vector3.up , out hit, 1.7f, layers);

		if(inCrawlSpace){
			print(hit.collider.gameObject.name);
		}
		
		return inCrawlSpace;
	}

	float TurnTowardsCrawlSpace(){
		// face the same direction as the trigger's forward as it will point towards the crawl space
		return specialMovementTrigger != null
			? specialMovementTrigger.transform.rotation.eulerAngles.y
			: transform.rotation.eulerAngles.y;
	}

	void SetCrawlHitbox(){
		controller.height = 0.5f;
		controller.center = new Vector3(0f,  0.5f / 2f, 0f);
	}

	void OnTriggerEnter(Collider col){
		if(this.enabled && col.tag.Equals("SpecialMovementTrigger")){
			if(col.GetComponent<SpecialMovementTriggers>().MovementType.Equals("crawl")){
				
			}
		}
	}

	void OnTriggerExit(Collider col){
		if(this.enabled && col.tag.Equals("SpecialMovementTrigger")){
			if(col.GetComponent<SpecialMovementTriggers>().MovementType.Equals("crawl") && !CheckIfCrawlSpace()){
				animator.SetBool("IsCrawling", false);
				GetComponent<PlayerControllerHandler>().ExitSpecialMovment();
			}
		}
	}

	#region debugging
	void OnDrawGizmos() {
		// var rayDown =  -1 * Vector3.up;
		// var climbCollider = climbHook.GetComponent<MeshFilter>(); 

         Gizmos.color = Color.red;
         //Gizmos.DrawRay(climbHook.transform.position, rayDown);
		// Gizmos.DrawRay(climbHook.transform.position + (climbCollider.sharedMesh.bounds.size.x / 40f) * Vector3.left, rayDown);
		// Gizmos.DrawRay(climbHook.transform.position + (climbCollider.sharedMesh.bounds.size.x / 40f) * Vector3.right, rayDown);

		if(specialMovementTrigger != null)
			Gizmos.DrawRay(specialMovementTrigger.transform.position, specialMovementTrigger.transform.forward * 5);

		//print ("Gizmos");
    }

	#endregion
}
