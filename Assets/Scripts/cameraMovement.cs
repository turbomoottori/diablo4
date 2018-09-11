﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour {

    public Transform player;
    public float camSpeed;
    Vector3 offset, target;
    float min, max, angle;
    float minFov, maxFov;

	void Start () {
        offset = transform.position - player.position;

        min = 7f;
        max = 15f;

        minFov = 50;
        maxFov = 100;
    }
	
	void Update () {
        //camera position
        Vector3 targetPos = player.position;
        Vector3 targ = targetPos + offset;
        Vector3 curr = transform.position;
        curr += (targ - curr) * Time.deltaTime * camSpeed;
        transform.position = curr;

        //camera rotation
        target = new Vector3(transform.position.x, player.transform.position.y, player.transform.position.z);
        var targetRot = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, (camSpeed / 2) * Time.deltaTime);

        if (Input.GetKey(KeyCode.Minus) && offset.y < max)
            offset.y += 0.1f;
        if (Input.GetKey(KeyCode.Period) && offset.y > min)
            offset.y -= 0.1f;

        //change FOV
        if(Input.GetKeyDown(KeyCode.L) && Camera.main.fieldOfView < maxFov)
            Camera.main.fieldOfView += 5;
        if(Input.GetKeyDown(KeyCode.K) && Camera.main.fieldOfView > minFov)
            Camera.main.fieldOfView -= 5;
	}
}
