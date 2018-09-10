using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class enemy : MonoBehaviour {

    //navmesh variables
    public float destRadius, maxTimer;
    float minTimer = 2f;
    float changeTimer;
    private float timer;
    private Transform target;
    protected UnityEngine.AI.NavMeshAgent agent;
    protected Rigidbody rb;
    public LayerMask mask;

    Vector3 lastPos, vel;
    protected bool isThrown, isAttacked, isGrounded;
    protected GameObject player, hpb, money;

    //hp variables
    UnityEngine.UI.Image hp;
    Transform hpPos;
    int enemyHealth;
    public int maxHP;
    float hpTimer = 5f;

    protected bool dying, hostile = false;

    public int minMoney, maxMoney;

    protected virtual void OnEnable () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        timer = maxTimer;
        //hpb = Instantiate(Resources.Load("enemyhealth", typeof(GameObject))) as GameObject;
        //hpb.transform.SetParent(GameObject.Find("Canvas").transform, false);
        money = Resources.Load<GameObject>("coin");
        //hp = hpb.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>(); 
	}

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        rb = GetComponent<Rigidbody>();
        enemyHealth = maxHP;
        hostile = false;
        lastPos = transform.position;
        changeTimer = Random.Range(minTimer, maxTimer);

    }

    public float Distance(Vector3 st, Vector3 en)
    {
        float d = 0.0f;

        float dx = st.x - en.x;
        float dy = st.y - en.y;
        float dz = st.z - en.z;

        d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);


        return d;
    }

    protected virtual void Update () {
        //timer to change destination
        timer += Time.deltaTime;
        if (timer >= changeTimer && !hostile)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector3 newPos = RandomDestination(transform.position, destRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }

        //ATTACK PLAYER
        if (hostile)
        {
            //disables interaction
            if (GetComponent<npcSpeech>() != null) {
                GetComponent<npcSpeech>().wantsToTalk = false;
            }

            //follows player
            if(agent.isOnNavMesh)
                agent.SetDestination(player.transform.position);
        }

        //SHOW HP BAR
        hpTimer += Time.deltaTime;
        if (hpTimer >= 2f && hpb != null)
            HideHP();

        //raycast to ground
        isGrounded = (Physics.Raycast(transform.position, Vector3.down, 1.3f, mask));

        //check movement direction
        vel = transform.position - lastPos;
        lastPos = transform.position;

        //jump and fall
        if(agent.isOnOffMeshLink)
        {
            if (vel.y > 0.1f)
            {
                //jump animation here
            } else if (vel.y < -0.1f)
            {
                //fall animation here
            }
        }

        //DYING
        if (enemyHealth <= 0)
            StartCoroutine(Dead(0.5f));

        //enemy health bar
        if (hpb != null)
        {
            hpb.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            hp.fillAmount = (float)enemyHealth / (float)maxHP;
        }
    }

    void HideHP()
    {
        hpb.SetActive(false);
    }

    void ShowHP()
    {
        hpTimer = 0;
        if (hpb == null)
        {
            hpb = Instantiate(Resources.Load("ui/enemyhealth") as GameObject, GameObject.Find("Canvas").transform);
            hp = hpb.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>();
        } else
        {
            hpb.SetActive(true);
        }
        //hpb.SetActive(true);
    }

    public static Vector3 RandomDestination(Vector3 origin, float distance, int layermask) {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;
        UnityEngine.AI.NavMeshHit navHit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }

    //dashed
    public IEnumerator Thrown(Vector3 playerPos)
    {
        if (isThrown)
            yield break;
        isThrown = true;
        hostile = true;
        Vector3 dir = transform.position - playerPos;
        dir.x *= 10;
        dir.z *= 10;
        dir.y = 75f;

        agent.enabled = false;
        rb.isKinematic = false;
        ShowHP();
        enemyHealth -= 5;
        rb.AddForce(dir, ForceMode.Impulse);
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isGrounded == true);
        rb.isKinematic = true;
        agent.enabled = true;
        isThrown = false;
    }

    //attacked with basic attack
    public IEnumerator Attacked(Vector3 playerPos, int w)
    {
        hostile = true;
        Vector3 dir = transform.position - playerPos;
        dir.x *= 10;
        dir.z *= 10;
        dir.y = 75f;
        agent.enabled = false;
        rb.isKinematic = false;
        ShowHP();

        if (isGrounded)
        {
            rb.AddForce(dir, ForceMode.Impulse);

            if (w == 1)
                enemyHealth -= weapons.damage1;
            else
                enemyHealth -= weapons.damage2;

        } else {
            rb.AddForce(dir * 3, ForceMode.Impulse);
            if (w == 1)
                enemyHealth -= (weapons.damage1 + 3);
            else
                enemyHealth -= (weapons.damage2 + 3);
        }

        if (isAttacked)
            yield break;
        isAttacked = true;
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isGrounded == true);
        rb.isKinematic = true;
        agent.enabled = true;
        isAttacked = false;
    }

    //attacked with stun attack
    public IEnumerator AttackStun(Vector3 playerPos, int w)
    {
        if (isAttacked)
            yield break;
        isAttacked = true;
        hostile = true;
        Vector3 dir = transform.position - playerPos;
        dir.x *= 10;
        dir.z *= 10;
        dir.y = 75f;
        agent.enabled = false;
        rb.isKinematic = false;
        ShowHP();

        if (w == 1)
            enemyHealth -= weapons.damage1;
        else
            enemyHealth -= weapons.damage2;

        rb.AddForce(dir * 1.5f, ForceMode.Impulse);
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isGrounded == true);
        print("stun");
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => isGrounded == true);
        print("stun ends");
        rb.isKinematic = true;
        agent.enabled = true;
        isAttacked = false;
    }

    public IEnumerator Shot(int dmg)
    {
        ShowHP();
        enemyHealth -= dmg;
        hostile = true;
        yield return null;
    }

    //dying
    public IEnumerator Dead(float time)
    {
        if (dying)
            yield break;
        dying = true;
        yield return new WaitUntil(() => isGrounded == true);
        Vector3 from = transform.localScale;
        Vector3 to = Vector3.zero;

        GameObject coin;

        for(var i = 0; i < Random.Range(minMoney,maxMoney); i++)
        {
            coin = Instantiate(money, transform.position + Vector3.up * 2, transform.rotation);
            coin.GetComponent<Rigidbody>().AddForce(Vector3.up, ForceMode.Impulse);
        }

        float t = 0f;
        while (t < time)
        {
            transform.localScale = Vector3.Slerp(from, to, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(hpb);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "sword")
        {
            if (playerMovement.attackNum == 1)
                StartCoroutine(Attacked(other.gameObject.transform.parent.position, playerMovement.activeWeapon));
            else if (playerMovement.attackNum == 2)
                StartCoroutine(AttackStun(other.gameObject.transform.parent.position, playerMovement.activeWeapon));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bullet")
        {
            StartCoroutine(Shot(collision.gameObject.GetComponent<bullet>().dmg));
            Destroy(collision.gameObject);
        }
    }
}
