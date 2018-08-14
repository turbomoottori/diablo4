
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
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

    public GameObject sword;
    float swordTime = 0.2f;
    bool swordActive;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        canMove = true;
    }

    void Update()
    {
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

        if (Input.GetButtonDown("Jump"))
        {
            canJump = (Physics.Raycast(transform.position, Vector3.down, 1.5f));

            if (canJump)
            {
                canJump = false;
                canDoubleJump = true;
                rb.velocity = new Vector3(0, jumpForce, 0);
            }
            else if (canDoubleJump)
            {
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
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (Vector3.Distance(transform.position, enemies[i].transform.position) <= 5f)
                    {
                        StartCoroutine(enemies[i].GetComponent<enemy>().Thrown(transform.position));
                    }
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

        //ATTACKING

        if (Input.GetButtonDown("Fire2"))
        {
            StartCoroutine(Attack(Vector3.up, 90f, swordTime));
        }
    }

    public enum DashState
    {
        Ready,
        Dashing,
        Cooldown
    }

    IEnumerator Attack(Vector3 axis, float angle, float time)
    {
        if (swordActive)
            yield break;
        swordActive = true;
        GameObject sw;
        sw = Instantiate(sword, transform.position, transform.rotation);
        sw.transform.parent = transform;
        sw.transform.Rotate(0, -45, 0, Space.Self);
        Quaternion from = sw.transform.rotation;
        Quaternion to = sw.transform.rotation;
        to *= Quaternion.Euler(axis * angle);
        float t = 0f;
        while (t < time)
        {
            sw.transform.rotation = Quaternion.Slerp(from, to, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        sw.transform.rotation = to;
        Destroy(sw);
        swordActive = false;
        yield return null;
    }
}