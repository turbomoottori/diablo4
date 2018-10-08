using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mutation : randomDestination {

    float notice = 4f;
    bool attacks;
    float jumpcd = 2f;
    float attackTimer;
    float maxAttackTimer = 3f;
    int randomAttack;
    BoxCollider atkBox;
    public int damage;

    protected override void Start()
    {
        base.Start();
        atkBox = transform.Find("attackBox").gameObject.GetComponent<BoxCollider>();
        atkBox.enabled = false;
        atkBox.gameObject.GetComponent<enemyAttack>().dmg = damage;
    }

    protected override void Update () {
        base.Update();

        float playerDistance = Distance(player.transform.position, agent.transform.position);
        if (playerDistance < notice && !hostile)
            hostile = true;

        if (!hostile)
        {
            if (player.GetComponent<playerMovement>().runs)
                Notice(2f);
            if (player.GetComponent<playerMovement>().dashes)
                Notice(3f);
        }
        else if(hostile)
        {
            if (playerDistance < 6f)
            {

                attackTimer += Time.deltaTime;
                if (attackTimer >= maxAttackTimer)
                {
                    randomAttack = Random.Range(0, 10);
                    attackTimer = 0;
                }

                //randomly chooses attack type
                if (randomAttack < 3 && !attacks)
                    StartCoroutine(JumpAttack(player.transform.position));
                if (randomAttack >= 3 && !attacks)
                    StartCoroutine(RunAttack(player.transform.position));
            }
        }
    }

    public void Notice(float noticeDistance)
    {
        if (!hostile)
        {
            float playerDistance = Distance(player.transform.position, agent.transform.position);
            if (playerDistance < (noticeDistance + notice))
                hostile = true;
        }
    }

    //jumps to player and deals damage
    IEnumerator JumpAttack(Vector3 playerPos)
    {
        if (attacks)
            yield break;
        attacks = true;
        agent.enabled = false;
        rb.isKinematic = false;
        Vector3 dir = playerPos - transform.position;
        transform.LookAt(new Vector3(playerPos.x,transform.position.y, playerPos.z));
        dir *= 15;
        dir.y = 75f;
        rb.AddForce(dir, ForceMode.Impulse);
        atkBox.enabled = true;
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isGrounded == true);
        atkBox.enabled = false;
        rb.isKinematic = true;
        yield return new WaitForSeconds(jumpcd);
        agent.enabled = true;
        yield return new WaitForSeconds(0.3f);
        attacks = false;
    }

    //runs to player and deals damage
    IEnumerator RunAttack(Vector3 playerPos)
    {
        if (attacks)
            yield break;
        attacks = true;
        agent.enabled = false;
        rb.isKinematic = false;
        transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
        rb.freezeRotation = true;
        print("run");
        rb.velocity = transform.forward * 25f;
        atkBox.enabled = true;
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => isGrounded == true);
        atkBox.enabled = false;
        agent.enabled = true;
        rb.isKinematic = true;
        attacks = false;
    }
}
