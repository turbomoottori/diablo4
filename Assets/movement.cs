using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class movement : MonoBehaviour {

    public float playerSpeed = 4f;
    private Vector2 direction;
    private Vector2 refVelocity;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector2 targetVelocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetVelocity.Normalize();
        GetComponent<Rigidbody2D>().velocity = targetVelocity * playerSpeed;

        if (GetComponent<Rigidbody2D>().velocity != Vector2.zero) {
            direction = GetComponent<Rigidbody2D>().velocity;
            transform.up = Vector2.SmoothDamp(transform.up, direction, ref refVelocity, 0.1f, Mathf.Infinity);
        }

    }
}
