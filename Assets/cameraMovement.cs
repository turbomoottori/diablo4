using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour {

    public Transform player;
    public float camSpeed;
    Vector3 offset;

	void Start () {
        offset = transform.position - player.position;
	}
	
	void Update () {
        Vector3 targetPos = player.position;
        Vector3 targ = targetPos + offset;
        Vector3 curr = transform.position;
        curr += (targ - curr) * Time.deltaTime * camSpeed;
        transform.position = curr;
	}
}
