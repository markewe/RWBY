using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeChecker : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
		var layerMask = 1 << (int)Layers.Environment;
        var collider = GetComponent<BoxCollider>();
		var rayLength = (transform.position - player.transform.position).y - (player.GetComponent<CharacterController>().height / 2f);
        var validLedge = true;

        // make sure there's enough space for player on ledge
        var rays = new List<Vector3>();

        //print(rayLength);

        rays.Add(transform.position + (collider.size.z * (transform.localScale.z / 2f)) * (transform.forward));
        //rays.Add(transform.position + (collider.size.z * (transform.localScale.x / 2f)) * (transform.forward * -1f));
        rays.Add(transform.position + (collider.size.x * (transform.localScale.x / 2f)) * (transform.right * -1f));
        rays.Add(transform.position + (collider.size.x * (transform.localScale.x / 2f)) * (transform.right));

        foreach(var ray in rays){
            if(!Physics.Raycast(ray, (transform.up * -1f), out hit, rayLength, layerMask)){
                validLedge = false;
                break;
            }
        }

        if(validLedge && Physics.Raycast(transform.position, (transform.up * -1f), out hit, rayLength, layerMask)){
            //print(hit.point.y - player.transform.position.y);
            player.GetComponent<PlayerInputHandler>().ledgeInfo = hit;
        }
    }

    #region debug

    void OnDrawGizmos() {
        var collider = GetComponent<BoxCollider>();
		Gizmos.color = Color.red;


        Gizmos.DrawRay(transform.position + (collider.size.z * (transform.localScale.z / 2f)) * (transform.forward), Vector3.up * -1f);
        Gizmos.DrawRay(transform.position, Vector3.up * -1f);
        Gizmos.DrawRay(transform.position + (collider.size.x * (transform.localScale.x / 2f)) * (transform.right * -1f), Vector3.up * -1f);
        Gizmos.DrawRay(transform.position + (collider.size.x * (transform.localScale.x / 2f)) * (transform.right), Vector3.up * -1f);
	}

    #endregion
}
