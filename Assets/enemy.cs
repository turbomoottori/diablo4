using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour {

    public float destRadius;
    public float maxTimer;
    private float timer;
    private Transform target;
    private UnityEngine.AI.NavMeshAgent agent;
    Rigidbody rb;
    public LayerMask mask;

    public bool isThrown;
    public bool isAttacked;
    bool isGrounded;
    GameObject player;

    void OnEnable () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        timer = maxTimer;
	}

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        rb = GetComponent<Rigidbody>();
    }

    void Update () {
        //timer to change destination
        timer += Time.deltaTime;
        if (timer >= maxTimer && !isThrown && !isAttacked)
        {
            Vector3 newPos = RandomDestination(transform.position, destRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        isGrounded = (Physics.Raycast(transform.position, Vector3.down, 1.3f, mask));
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

        Vector3 dir = transform.position - playerPos;
        dir.x *= 10;
        dir.z *= 10;
        dir.y = 75f;

        agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(dir, ForceMode.Impulse);
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isGrounded == true);
        rb.isKinematic = true;
        agent.enabled = true;
        isThrown = false;
    }

    public IEnumerator Attacked(Vector3 playerPos)
    {
        Vector3 dir = transform.position - playerPos;
        dir.x *= 10;
        dir.z *= 10;
        dir.y = 75f;

        agent.enabled = false;
        rb.isKinematic = false;

        if (isGrounded)
        {
            rb.AddForce(dir, ForceMode.Impulse);
        } else {
            rb.AddForce(dir * 3, ForceMode.Impulse);
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
        Vector3 dir = transform.position - playerPos;
        dir.x *= 10;
        dir.z *= 10;
        dir.y = 75f;

        agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(dir * 1.5f, ForceMode.Impulse);
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isGrounded == true);
        print("stun");
        yield return new WaitForSeconds(2f);
        print("stun ends");
        rb.isKinematic = true;
        agent.enabled = true;
        isAttacked = false;
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
