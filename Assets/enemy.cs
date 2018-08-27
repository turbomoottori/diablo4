using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour {

    public float destRadius, maxTimer;
    private float timer;
    private Transform target;
    private UnityEngine.AI.NavMeshAgent agent;
    Rigidbody rb;
    public LayerMask mask;

    public bool isThrown, isAttacked;
    bool isGrounded;
    GameObject player;

    GameObject hpb;
    UnityEngine.UI.Image hp;
    Transform hpPos;
    int enemyHealth, maxHP;
    float hpTimer;
    bool dying = false;

    bool swordActive = false;
    GameObject sword;
    bool hostile;

    GameObject money;
    public int minMoney, maxMoney;

    void OnEnable () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        maxHP = 50;
        timer = maxTimer;
        hpb = Instantiate(Resources.Load("enemyhealth", typeof(GameObject))) as GameObject;
        hpb.transform.SetParent(GameObject.Find("Canvas").transform, false);
        money = Resources.Load<GameObject>("coin");
        sword = Resources.Load<GameObject>("sword");
        hpTimer = 5;
        hp = hpb.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>(); 
	}

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        rb = GetComponent<Rigidbody>();
        enemyHealth = maxHP;
        hostile = false;
    }

    float Distance(Vector3 st, Vector3 en)
    {
        float d = 0.0f;

        float dx = st.x - en.x;
        float dy = st.y - en.y;
        float dz = st.z - en.z;

        d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);


        return d;
    }

    void Update () {
        //timer to change destination
        timer += Time.deltaTime;
        if (timer >= maxTimer && !hostile)
        {
            Vector3 newPos = RandomDestination(transform.position, destRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        //ATTACK PLAYER

        if (hostile)
        {
            if (GetComponent<npcSpeech>() != null) {
                GetComponent<npcSpeech>().wantsToTalk = false;
            }
            agent.SetDestination(player.transform.position);

            float dist = Distance(player.transform.position, agent.transform.position);
            
            if (dist <= 1.6f)
            {
                StartCoroutine(Attack(Vector3.up, 90f, 0.5f));
            } 
        }

        //SHOW HP BAR

        hpTimer += Time.deltaTime;
        if (hpTimer >= 2f)
            HideHP();

        isGrounded = (Physics.Raycast(transform.position, Vector3.down, 1.3f, mask));

        if (enemyHealth <= 0)
            StartCoroutine(Dead(0.5f));

        hpb.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        hp.fillAmount = (float)enemyHealth / (float)maxHP;
    }

    void HideHP()
    {
        hpb.SetActive(false);
    }

    void ShowHP()
    {
        hpTimer = 0;
        hpb.SetActive(true);
    }

    public static Vector3 RandomDestination(Vector3 origin, float distance, int layermask) {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;
        UnityEngine.AI.NavMeshHit navHit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }

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

    public IEnumerator Attacked(Vector3 playerPos)
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
            enemyHealth -= 5;
        } else {
            rb.AddForce(dir * 3, ForceMode.Impulse);
            enemyHealth -= 10;
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

    public IEnumerator AttackStun(Vector3 playerPos)
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
        enemyHealth -= 10;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "sword")
        {
            if (player.GetComponent<playerMovement>().attackNum == 1)
               StartCoroutine(Attacked(other.gameObject.transform.parent.position));
            else if (player.GetComponent<playerMovement>().attackNum == 2)
                StartCoroutine(AttackStun(other.gameObject.transform.parent.position));
        }
    }
}
