using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour {

    public float speed;
    public float rotationSpeed;
    public float jumpForce;
    public float dashForce;

    private Rigidbody rb;
    Vector3 refVelocity;
    RaycastHit hit;

    bool canJump;
    bool canDoubleJump;
    bool canMove;

    DashState dashState;
    float dashTimer;
    public float maxDash = 20f;
    Vector3 savedVelocity;

    void Start () {
        rb = GetComponent<Rigidbody>();
        canMove = true;
	}
	
	void Update () {

        //MOVEMENT
        if (canMove)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
            movement.Normalize();
            rb.MovePosition(transform.position + movement * speed);
        }

        //LOOKAT MOVEMENT DIRECTION

        /*if (movement != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }*/

        //LOOKAT MOUSE POSITION

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane hPlane = new Plane(Vector3.up, transform.position);
        float distance = 0;
        if (hPlane.Raycast(ray, out distance))
        {
            transform.LookAt(ray.GetPoint(distance));
        }

       

        //JUMP AND DOUBLE JUMP

        if (Input.GetButtonDown("Jump")) {
            canJump = (Physics.Raycast(transform.position, Vector3.down, 1f));

            if (canJump)
            {
                canJump = false;
                canDoubleJump = true;
                rb.velocity = new Vector3(0, jumpForce, 0);
            }
            else if (canDoubleJump) {
                canDoubleJump = false;
                rb.velocity = new Vector3(0, jumpForce, 0);
            }
        }

        //DASHING

        switch (dashState)
        {
            case DashState.Ready:
                var isDashKeyDown = Input.GetButtonDown("Fire1");
                if (isDashKeyDown)
                {
                    savedVelocity = rb.velocity;
                    rb.velocity = transform.forward * dashForce;
                    dashState = DashState.Dashing;
                }
                break;
            case DashState.Dashing:
                canMove = false;
                dashTimer += Time.deltaTime * 3;
                if (dashTimer >= maxDash)
                {
                    dashTimer = maxDash;
                    rb.velocity = savedVelocity;
                    dashState = DashState.Cooldown;
                }
                break;
            case DashState.Cooldown:
                canMove = true;
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    dashTimer = 0;
                    dashState = DashState.Ready;
                }
                break;
        }


    }

    public enum DashState
    {
        Ready,
        Dashing,
        Cooldown
    }

    private void OnCollisionStay(Collision collision)
    {
        if (dashState == DashState.Dashing && collision.gameObject.tag=="enemy") {
            print("jees");
        }
    }

}
