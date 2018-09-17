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
	Vector3 hitboxPosition = new Vector3(0f, 0f, 0.5f);
	float targetRotation;
	float currentSpeed;
	float targetSpeed;
	float speedSmoothVelocity;
	float turnSmoothTime = 5f;
	float cameraTurnSpeed = 1f;
	static float crawlSpeedSmoothTime = 0.1f;
	static float crawlSpeed = 2f;

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
		float mouseX = Input.GetAxis("Mouse X") * cameraTurnSpeed;
		var rotation = transform.rotation.eulerAngles;

		rotation.y += mouseX;
		transform.rotation = Quaternion.Euler(rotation);
		targetDirection = new Vector3(inputX, 0f, inputZ);

		var targetSpeed = crawlSpeed * targetDirection.magnitude;
		var smoothTime = crawlSpeedSmoothTime;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
		Vector3 vel = (transform.forward * currentSpeed * inputZ) + (transform.right * currentSpeed * inputX);

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
		inCrawlSpace = Physics.Raycast(transform.position, Vector3.up , out hit, 1.7f, layers);
		
		return inCrawlSpace;
	}

	void SetCrawlHitbox(){
		controller.height = 0.5f;
		controller.center = new Vector3(0f,  0.5f / 2f, 0f) + hitboxPosition;
	}

	void OnTriggerEnter(Collider col){
		if(this.enabled && col.tag.Equals("SpecialMovementTrigger")){
			if(col.GetComponent<SpecialMovementTriggers>().movementType.Equals("crawl")){
				
			}
		}
	}

	void OnTriggerExit(Collider col){
		if(this.enabled && col.tag.Equals("SpecialMovementTrigger")){
			if(col.GetComponent<SpecialMovementTriggers>().movementType.Equals("crawl") && !CheckIfCrawlSpace()){
				animator.SetBool("IsCrawling", false);
				GetComponent<PlayerControllerHandler>().ExitSpecialMovment("crawl");
			}
		}
	}

	#region debugging
	void OnDrawGizmos() {

    }

	#endregion
}
