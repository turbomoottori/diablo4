using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour {

    public Transform player;
    public float camSpeed;
    Vector3 offset;

	// Use this for initialization
	void Start () {
        offset = transform.position - player.position;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 targetPos = player.position;
        Vector3 targ = targetPos + offset;
        Vector3 curr = transform.position;
        curr += (targ - curr) * Time.deltaTime * camSpeed;
        transform.position = curr;
	}
}
