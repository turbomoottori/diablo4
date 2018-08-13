using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour {

    public float destRadius;
    public float maxTimer;
    private Transform target;
    private UnityEngine.AI.NavMeshAgent agent;
    private float timer;

	// Use this for initialization
	void OnEnable () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        timer = maxTimer;
	}
	
	// Update is called once per frame
	void Update () {

        //timer to change destination
        timer += Time.deltaTime;
        if (timer >= maxTimer)
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

}
