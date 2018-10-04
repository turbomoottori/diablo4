using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guard : enemy {

    public GameObject[] patrolPoints;
    int patrolNum = 0;
    float timer = 0;
    public Action action;
    public float stopTimer;
    public string type, special;
    public float speed, range, rlSpeed;
    public int damage, bullets;
    private int bulletCount;
    float shootcd;
	
	protected override void Update () {
        base.Update();

        Vector3 targetDir = player.transform.position - transform.position;
        float angleToPlayer = Vector3.Angle(targetDir, transform.forward);

        //checks if player is on sight
        if(angleToPlayer >= -70 && angleToPlayer <= 70)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, targetDir, out hit, 10f))
            {
                if (hit.collider.gameObject.tag == "player")
                {
                    hostile = true;
                }
            }
        }

        if (hostile)
        {
            //look at player more accurately
            Vector3 target = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            var targetRot = Quaternion.LookRotation(target - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 7 * Time.deltaTime);
        }

        switch (action)
        {
            //sets new destination
            case Action.newPoint:
                timer += Time.deltaTime;
                if (timer >= stopTimer && !hostile)
                {
                    agent.SetDestination(patrolPoints[patrolNum].transform.position);
                    action = Action.walkToPoint;
                }
                if (hostile)
                    action = Action.follow;
                break;

            //goes to destination and chooses a new one
            case Action.walkToPoint:
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (patrolNum < (patrolPoints.Length - 1))
                        patrolNum += 1;
                    else
                        patrolNum = 0;

                    timer = 0;
                    action = Action.newPoint;
                }
                if (hostile)
                    action = Action.follow;
                break;

            //checks if guard is close enough to player
            case Action.follow:
                float dist = Distance(player.transform.position, agent.transform.position);

                if (dist <= 6f)
                    action = Action.chooseAttack;
                break;

            //randomly chooses an attack
            case Action.chooseAttack:
                int actionNum = Random.Range(1, 3);

                if (actionNum == 1)
                    action = Action.shoot;
                else
                    action = Action.special;
                break;

            //basic attack
            case Action.shoot:
                Vector3 bPos = transform.position + transform.forward;
                GameObject b = Instantiate(Resources.Load<GameObject>("bullet"));
                b.transform.position = bPos;
                b.tag = "enemyBullet";
                b.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                b.GetComponent<bullet>().maxRange = range;
                b.GetComponent<bullet>().dmg = damage;

                bulletCount += 1;
                action = Action.cooldown;
                break;

            //special attack
            case Action.special:
                //"big bullet", 4 times damage, long cooldown
                if (special == "big")
                {
                    bPos = transform.position + transform.forward;
                    b = Instantiate(Resources.Load<GameObject>("bullet"));
                    b.transform.position = bPos;
                    b.transform.localScale *= 2;
                    b.tag = "enemyBullet";
                    b.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                    b.GetComponent<bullet>().maxRange = range;
                    b.GetComponent<bullet>().dmg = damage * 4;

                    bulletCount += bullets;
                    shootcd *= 2f;
                    action = Action.cooldown;
                }
                break;

            //cooldown
            case Action.cooldown:
                if (bulletCount >= bullets)
                {
                    shootcd -= Time.deltaTime;
                    if (shootcd <= 0)
                    {
                        shootcd = rlSpeed;
                        bulletCount = 0;
                        action = Action.follow;
                    }
                }
                else
                {
                    action = Action.follow;
                }
                break;
        }
	}

    public enum Action
    {
        newPoint,
        walkToPoint,
        follow,
        chooseAttack,
        shoot,
        special,
        cooldown
    }
}
