
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float jumpForce;
    public float dashForce;

    public int health;
    UnityEngine.UI.Image healthbar;

    private Rigidbody rb;
    Vector3 refVelocity;
    RaycastHit hit;

    bool canJump;
    bool canDoubleJump;
    bool canMove;

    //dash variables
    DashState dashState;
    float dashTimer;
    public float maxDash = 20f;
    Vector3 savedVelocity;

    //attack variables
    public GameObject sword;
    float swordTime = 0.2f;
    float swordSpinTime = 0.5f;
    bool swordActive;
    public int attackNum;

    //slow motion variables
    bool slowTime, slowmocd;
    Vector3 gravForce = Vector3.down * 500;
    UnityEngine.UI.Image slowmobar;

    ConstantForce fakeGrav;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        canMove = true;
        slowTime = false;
        fakeGrav = GetComponent<ConstantForce>();
        slowmobar = GameObject.Find("SlowMoBar").GetComponent<UnityEngine.UI.Image>();
        healthbar = GameObject.Find("Health").GetComponent<UnityEngine.UI.Image>();
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
            Vector3 pos = transform.position;
            Vector3 targ = transform.position + movement;

            //check if the player runs or walks
            if (Input.GetKey(KeyCode.LeftShift)) {
                pos += (targ - pos) * Time.deltaTime * (speed * 1.5f);
            }
            else {
                pos += (targ - pos) * Time.deltaTime * speed;
            }
            transform.position = pos;
        }

        //LOOKAT MOVEMENT DIRECTION

        /*if (movement != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }*/

        //LOOKAT MOUSE POSITION

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane hPlane = new Plane(Vector3.up, transform.position);
        float direction = 0;
        if (hPlane.Raycast(ray, out direction))
        {
            transform.LookAt(ray.GetPoint(direction));
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
                var isDashKeyDown = Input.GetKeyDown(KeyCode.Alpha2);
                if (isDashKeyDown)
                {
                    if (slowTime)
                        fakeGrav.relativeForce = Vector3.zero;
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
                if (slowTime)
                    fakeGrav.relativeForce = gravForce;
                else
                    fakeGrav.relativeForce = Vector3.zero;
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    dashTimer = 0;
                    dashState = DashState.Ready;
                }
                break;
        }

        //ATTACKING

        if (Input.GetButtonDown("Fire1") && !swordActive)
        {
            StartCoroutine(Attack(Vector3.up, 90f, swordTime));
        }

        // SPIN ATTACK 

        if (Input.GetButtonDown("Fire2") && !swordActive)
        {
            StartCoroutine(AttackTwo(Vector3.up, swordSpinTime));
        }

        //SLOW TIME

        if (Input.GetKeyDown(KeyCode.Alpha1) && !slowmocd)
        {
            StartCoroutine(SlowTime(3f, 6f));
        }
    }

    public enum DashState
    {
        Ready,
        Dashing,
        Cooldown
    }

    IEnumerator SlowTime(float time, float cooldownTime)
    {
        //set slow motion
        slowTime = true;
        slowmocd = true;
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        speed *= 2;
        fakeGrav.relativeForce = gravForce;
        jumpForce *= 2;
        swordTime /= 2;
        swordSpinTime /= 2;

        //duration 
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            slowmobar.fillAmount = 1 - t / time;
            yield return null;
        }
        slowTime = false;

        //set normal speed
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        speed /= 2;
        fakeGrav.relativeForce = Vector3.zero;
        jumpForce /= 2;
        swordTime *= 2;
        swordSpinTime *= 2;

        //cooldown
        t = 0f;
        while (t < cooldownTime)
        {
            t += Time.deltaTime;
            slowmobar.fillAmount = t / cooldownTime;
            yield return null;
        }

        slowmocd = false;
        yield return null;
    }

    //spawns, animates and destroys sword placeholder
    IEnumerator Attack(Vector3 axis, float angle, float time)
    {
        if (swordActive)
            yield break;
        swordActive = true;
        attackNum = 1;
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

    IEnumerator AttackTwo(Vector3 axis, float time)
    {
        if (swordActive)
            yield break;
        swordActive = true;
        attackNum = 2;
        GameObject sw;
        sw = Instantiate(sword, transform.position, transform.rotation);
        sw.transform.parent = transform;
        float from = transform.eulerAngles.y;
        float to = from + 360f;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float yRot = Mathf.Lerp(from, to, t / time) % 360f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRot, transform.eulerAngles.z);
            yield return null;
        }
        Destroy(sw);
        swordActive = false;
        yield return null;
    }
}