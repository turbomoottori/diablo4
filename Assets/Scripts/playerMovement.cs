
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{
    public float speed, rotationSpeed, jumpForce, dashForce;
    public bool runs, dashes;
    bool canJump, canDoubleJump, canMove;

    private Rigidbody rb;
    Vector3 refVelocity;
    RaycastHit hit;

    //dash variables
    public DashState dashState;
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
    int money = 0;
    int moneyValue = 10;
    Text moneyui;
    bool uiActive = false;

    Transform canv;

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
    }

    void Update()
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
                Vector3 pos = transform.position;
                Vector3 targ = transform.position + movement;

                //check if the player runs or walks
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    pos += (targ - pos) * Time.deltaTime * (speed * 1.5f);
                    runs = true;
                }
                else
                {
                    pos += (targ - pos) * Time.deltaTime * speed;
                    runs = false;
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
                else if (canDoubleJump && gameControl.control.knowsDoubleJump)
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
                    dashes = true;
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
                print("sldgdkj");
                StartCoroutine(SlowTime(3f, 6f));
            }
        }
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
            moneyui = Instantiate(Resources.Load("ui/money") as GameObject, canv).GetComponent<Text>();
            moneyui.text = money.ToString();
        } else
        {
            moneyui.gameObject.SetActive(true);
        }

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            moneyui.text = tempMoney.ToString() + " + " + (money - tempMoney).ToString();
            yield return null;
        }
        moneyui.text = money.ToString();
        yield return new WaitForSeconds(2f);
        moneyui.gameObject.SetActive(false);
        uiActive = false;
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemysword")
        {
            if (other.gameObject.transform.parent.parent.GetComponent<civilian>().attackNum == 1)
            {
                gameControl.control.hp -= 1;
                print("regular attack");
            } else if (other.gameObject.transform.parent.parent.GetComponent<civilian>().attackNum == 2)
            {
                gameControl.control.hp -= 2;
                print("stun attack");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "coin")
        {
            StartCoroutine(AddMoney(money, 3f));
            money += moneyValue;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "enemyBullet")
        {
            gameControl.control.hp -= collision.gameObject.GetComponent<bullet>().dmg;
            Destroy(collision.gameObject);
        }
    }
}