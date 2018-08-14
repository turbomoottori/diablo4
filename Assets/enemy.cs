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

    public bool isThrown;
    public bool isAttacked;

	void OnEnable () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        timer = maxTimer;
	}

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {
        //timer to change destination
        timer += Time.deltaTime;
        if (timer >= maxTimer && !isThrown && !isAttacked)
        {
            Vector3 newPos = RandomDestination(transform.position, destRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
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
        yield return new WaitForSeconds(1f);
        rb.isKinematic = true;
        agent.enabled = true;
        isThrown = false;
    }

    public IEnumerator Attacked(Vector3 playerPos)
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
        rb.AddForce(dir, ForceMode.Impulse);
        yield return new WaitForSeconds(1f);
        rb.isKinematic = true;
        agent.enabled = true;
        isAttacked = false;
    }
}
