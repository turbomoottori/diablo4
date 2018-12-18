using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class importantNpc : MonoBehaviour {

    NavMeshAgent agent;
    public LayerMask mask;
    float minTimer = 2f;
    public float destRadius, maxTimer;
    float timer, changeTimer;
    GameObject player;
    public Animator anim;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
        //timer to change destination
        timer += Time.deltaTime;
        if (timer >= changeTimer)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                changeTimer = Random.Range(minTimer, maxTimer);
                Vector3 newPos = RandomDestination(transform.position, destRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }

        if (agent.isActiveAndEnabled)
        {
            if (anim != null)
            {
                if (agent.velocity != Vector3.zero)
                    anim.SetBool("movement", true);
                else
                    anim.SetBool("movement", false);
            }

            if (gameObject.GetComponent<dialogue_npc>() != null && Vector3.Distance(transform.position, player.transform.position)< 2f)
                agent.isStopped = true;
            else
                agent.isStopped = false;
        }
    }

    public static Vector3 RandomDestination(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }
}
