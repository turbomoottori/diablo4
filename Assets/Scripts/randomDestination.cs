using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomDestination: enemy {

    private float timer, changeTimer;
    float minTimer = 2f;
    public float destRadius, maxTimer;

    protected override void Start()
    {
        base.Start();
        timer = maxTimer;
        changeTimer = Random.Range(minTimer, maxTimer);
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();

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
    }

    public static Vector3 RandomDestination(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;
        UnityEngine.AI.NavMeshHit navHit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }
}
