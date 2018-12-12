
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{
    public Animator anim;
    public float speed, rotationSpeed, jumpForce, dashForce;
    public bool runs, dashes;
    bool canDoubleJump, canMove;
    int jumpCounter = 0;
    public static Vector3 savedPos;
    public static float speedmodifier = 0;

    private Rigidbody rb;
    Vector3 refVelocity;

    //dash variables
    public static DashState dashState;
    float dashTimer;
    public float maxDash = 20f;
    Vector3 savedVelocity;

    //slow motion variables
    public static bool slowTime;
    bool slowmocd;
    Vector3 gravForce = Vector3.down * 500;
    Image slowmobar;

    ConstantForce fakeGrav;
    public static bool paused = false;

    //money variables
    int moneyValue = 10;
    GameObject moneyui;
    bool uiActive = false;

    Transform canv;
    public LayerMask walkable;
    bool respawn, hurt = false;

    void Start()
    {
        if (gameControl.control.knowsDash == false)
            dashState = DashState.Off;
        else
            dashState = DashState.Ready;

        gameControl.control.maxhp = 50;
        gameControl.control.hp = 50;

        canv = GameObject.Find("Canvas").transform;

        rb = GetComponent<Rigidbody>();
        canMove = true;
        slowTime = false;
        fakeGrav = GetComponent<ConstantForce>();
        slowmobar = Instantiate(Resources.Load("ui/slowmo") as GameObject, canv).transform.GetChild(0).GetChild(0).transform.GetComponent<Image>();

        if (gameControl.control.knowsDash)
            slowmobar.gameObject.SetActive(true);
        else
            slowmobar.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!paused)
        {
            anim.SetFloat("y_velocity", rb.velocity.y);
            if (grounded())
            {
                if(Physics.Raycast(transform.position, Vector3.down,0.5f, walkable))
                    savedPos = transform.position;

                anim.SetBool("grounded", true);
                jumpCounter = 0;
            }

            if (Input.GetButtonDown("Jump"))
            {
                print("jump button pressed");
                if (grounded())
                {
                    savedPos = transform.position;
                    jumpCounter += 1;
                    anim.SetTrigger("jump");
                    rb.velocity = new Vector3(0, jumpForce, 0);
                }
                else
                {
                    if (jumpCounter < 1 && gameControl.control.knowsDoubleJump)
                    {
                        jumpCounter += 1;
                        anim.SetTrigger("jump");
                        rb.velocity = new Vector3(0, jumpForce, 0);
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!paused)
        {
            //MOVEMENT
            if (canMove)
            {
                float moveHorizontal = Input.GetAxis("Horizontal");
                float moveVertical = Input.GetAxis("Vertical");
                Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
                movement.Normalize();
                if (movement != Vector3.zero)
                    anim.SetBool("movement", true);
                else
                    anim.SetBool("movement", false);

                //LOOK AT MOVEMENT DIRECTION
                if (movement != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }

                //prevents being stuck on wall
                RaycastHit hit;
                if (Physics.Raycast(transform.position+Vector3.up, movement, out hit, 1.4f))
                    movement = Vector3.zero;

                Vector3 pos = transform.position;
                Vector3 targ = transform.position + movement;
                pos += (targ - pos) * Time.deltaTime * (speed - speedmodifier);
                transform.position = pos;
                transform.position = pos;

                Vector3 localDir = transform.InverseTransformDirection(movement);
                anim.SetFloat("movement_forward", localDir.z);
                anim.SetFloat("movement_sideways", localDir.x);
            }
            /*
            //LOOKAT MOUSE POSITION
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane hPlane = new Plane(Vector3.up, transform.position);
            float direction = 0;
            if (hPlane.Raycast(ray, out direction))
            {
                Vector3 target = ray.GetPoint(direction);
                var targetRot = Quaternion.LookRotation(target - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10 * Time.deltaTime);
            }*/

            //DASHING

            switch (dashState)
            {
                case DashState.Ready:
                    var isDashKeyDown = Input.GetKeyDown(KeyCode.LeftShift);
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
                    dashes = true;
                    anim.SetTrigger("dash");
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
                        if (Vector3.Distance(transform.position, enemies[i].transform.position) <= 3f)
                        {
                            StartCoroutine(enemies[i].GetComponent<enemy>().Thrown(transform.position));
                        }
                    }
                    break;
                case DashState.Cooldown:
                    canMove = true;
                    dashes = false;
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
                case DashState.Off:
                    break;
            }

            //SLOW TIME

            if (Input.GetKeyDown(KeyCode.Alpha1) && !slowmocd && !paused && gameControl.control.knowsSlowTime)
            {
                StartCoroutine(SlowTime(3f, 6f));
            }
        }

        //load autosave if dead
        if (gameControl.control.hp <= 0)
            gameControl.control.AutoLoad();
    }

    public enum DashState
    {
        Ready,
        Dashing,
        Cooldown,
        Off
    }

    //slows time
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
        attack.swordTime /= 2;
        attack.swordSpinTime /= 2;

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
        attack.swordTime *= 2;
        attack.swordSpinTime *= 2;

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

    //SHOW AND UPDATE MONEY UI
    
    IEnumerator AddMoney(int tempMoney, float time)
    {
        if (uiActive)
            yield break;
        uiActive = true;

        //creates money ui if it doesn't exist yet
        if (moneyui == null)
        {
            moneyui = Instantiate(Resources.Load("ui/money") as GameObject, canv, false);
            moneyui.transform.GetChild(0).GetComponent<Text>().text = gameControl.control.money.ToString();
        } else
        {
            moneyui.gameObject.SetActive(true);
        }

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            moneyui.transform.GetChild(0).GetComponent<Text>().text = tempMoney.ToString() + " + " + (gameControl.control.money - tempMoney).ToString();
            yield return null;
        }
        moneyui.transform.GetChild(0).GetComponent<Text>().text = gameControl.control.money.ToString();
        yield return new WaitForSeconds(2f);
        moneyui.gameObject.SetActive(false);
        uiActive = false;
        yield return null;
    }

    IEnumerator Fall(float time, Vector3 pos)
    {
        respawn = true;
        float t = 0f;
        Vector3 from = transform.localScale;
        Vector3 to = Vector3.zero;

        //prevents from spawning too close to the edge
        Vector3 d = pos - transform.position;
        d.y = 0;
        if (d.z > 3)
            d.z = 3;
        else if (d.z < -3)
            d.z = -3;
        if (d.x > 3)
            d.x = 3;
        else if (d.x < -3)
            d.x = -3;

        while (t < time)
        {
            transform.localScale = Vector3.Slerp(from, to, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = from;
        transform.position = pos + d;
        respawn = false;
    }

    bool grounded()
    {
        Vector3 size = new Vector3(0.7f, 0.6f, 0.7f);
        Collider[] collides = Physics.OverlapBox(transform.position, size, Quaternion.identity, walkable);
        if (collides.Length > 0)
            return true;
        return false;
    }

    //display slow motion bar
    public void ShowSlowMo()
    {
        slowmobar.gameObject.SetActive(true);
    }

    //when enemy hits
    public IEnumerator Hurt(int dmg)
    {
        if (hurt)
            yield return null;

        hurt = true;
        gameControl.control.hp -= dmg;
        //animation and sound here
        yield return new WaitForSeconds(0.5f); //change to match animation
        hurt = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "fall" && !respawn)
            StartCoroutine(Fall(1f, savedPos));

        if (other.gameObject.tag == "enemysword" && !hurt)
            StartCoroutine(Hurt(other.gameObject.GetComponent<enemyAttack>().dmg));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "coin")
        {
            StartCoroutine(AddMoney(gameControl.control.money, 3f));
            gameControl.control.money += moneyValue;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "enemyBullet")
        {
            gameControl.control.hp -= collision.gameObject.GetComponent<bullet>().dmg;
            Destroy(collision.gameObject);
        }
    }
}