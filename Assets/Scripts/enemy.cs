using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class enemy : MonoBehaviour {

    //navmesh variables
    private Transform target;
    protected UnityEngine.AI.NavMeshAgent agent;
    protected Rigidbody rb;
    public LayerMask mask;
    private Vector3 startPos;

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
        money = Resources.Load<GameObject>("coin");
        startPos = transform.position;
	}

    protected virtual void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        enemyHealth = maxHP;
        hostile = false;
        lastPos = transform.position;

        foreach(EnemyList e in gameControl.control.enemies)
            if(e.posX == startPos.x && e.posY == startPos.y && e.posZ == startPos.z)
                Destroy(this.gameObject);
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
        //ATTACK PLAYER
        if (hostile)
        {
            //disables interaction
            if (GetComponent<interactable>() != null) {
                GetComponent<interactable>().HideE();
                Destroy(GetComponent<interactable>());
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
    }

    //dashed
    public IEnumerator Thrown(Vector3 playerPos)
    {
        if (isThrown || dying)
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
    public IEnumerator Attacked(Vector3 playerPos, int w, bool battery)
    {
        hostile = true;

        int dmg;
        if (w == 1)
            dmg = items.equippedOne.damage;
        else
            dmg = items.equippedTwo.damage;

        if (battery)
            dmg *= 2;

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
            enemyHealth -= dmg;

        } else {
            rb.AddForce(dir * 3, ForceMode.Impulse);
            enemyHealth -= dmg + 2;
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
    public IEnumerator AttackStun(Vector3 playerPos, int w, bool battery)
    {
        if (isAttacked)
            yield break;
        isAttacked = true;
        hostile = true;

        int dmg;
        if (w == 1)
            dmg = items.equippedOne.damage;
        else
            dmg = items.equippedTwo.damage;

        if (battery)
            dmg *= 2;

        Vector3 dir = transform.position - playerPos;
        dir.x *= 10;
        dir.z *= 10;
        dir.y = 75f;
        agent.enabled = false;
        rb.isKinematic = false;
        ShowHP();

        enemyHealth -= dmg;

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

        if (GetComponent<dropItem>() != null)
            GetComponent<dropItem>().SpawnItem();

        float t = 0f;
        while (t < time)
        {
            transform.localScale = Vector3.Slerp(from, to, t / time);
            t += Time.deltaTime;
            yield return null;
        }

        gameControl.control.enemies.Add(new EnemyList() { posX = startPos.x, posY = startPos.y, posZ = startPos.z });

        Destroy(hpb);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "sword")
        {
            bool battery = items.batteryOn();
            if (attack.attackNum == 1)
                StartCoroutine(Attacked(other.gameObject.transform.parent.position, attack.activeWeapon, battery));
            else if (attack.attackNum == 2)
                StartCoroutine(AttackStun(other.gameObject.transform.parent.position, attack.activeWeapon, battery));
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
