
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float speed, rotationSpeed, jumpForce, dashForce;
    public bool runs, dashes;

    int health, maxHP;
    UnityEngine.UI.Image healthbar;

    private Rigidbody rb;
    Vector3 refVelocity;
    RaycastHit hit;

    bool canJump, canDoubleJump, canMove;

    //dash variables
    public DashState dashState;
    float dashTimer;
    public float maxDash = 20f;
    Vector3 savedVelocity;

    //attack variables
    GameObject sword;
    float swordTime = 0.2f;
    float swordSpinTime = 0.5f;
    bool swordActive;
    public int attackNum;

    //slow motion variables
    public bool slowTime;
    bool slowmocd;
    Vector3 gravForce = Vector3.down * 500;
    UnityEngine.UI.Image slowmobar;

    ConstantForce fakeGrav;
    public bool paused = false;

    //money variables
    int money = 0;
    int moneyValue = 10;
    UnityEngine.UI.Text moneyui;
    bool uiActive = false;

    private void Awake()
    {
        maxHP = 50;
        health = maxHP;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        canMove = true;
        slowTime = false;
        fakeGrav = GetComponent<ConstantForce>();
        slowmobar = GameObject.Find("SlowMoBar").GetComponent<UnityEngine.UI.Image>();
        healthbar = GameObject.Find("Health").GetComponent<UnityEngine.UI.Image>();
        moneyui = GameObject.Find("MoneyText").GetComponent<UnityEngine.UI.Text>();
        sword = Resources.Load<GameObject>("sword");
        moneyui.text = money.ToString();
        moneyui.gameObject.SetActive(false);
    }

    void Update()
    {
        // HEALTH

        healthbar.fillAmount = (float)health / (float)maxHP;
        if (health <= 0)
        {
            print("kuolee");
        }

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

            if (Input.GetKeyDown(KeyCode.Alpha1) && !slowmocd && !paused)
            {
                StartCoroutine(SlowTime(3f, 6f));
            }
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

    //SHOW AND UPDATE MONEY UI

    IEnumerator AddMoney(int tempMoney, float time)
    {
        if (uiActive)
            yield break;
        uiActive = true;
        moneyui.gameObject.SetActive(true);
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
                health -= 1;
                print("regular attack");
            } else if (other.gameObject.transform.parent.parent.GetComponent<civilian>().attackNum == 2)
            {
                health -= 2;
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
    }
}