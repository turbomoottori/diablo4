﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class civilian : randomDestination {

    Animator anim;
    GameObject sword;
    bool swordActive = false;
    public Action action = Action.follow;
    public int damage;

    protected override void OnEnable()
    {
        base.OnEnable();
        sword = Resources.Load<GameObject>("enemysword");
        sword.GetComponentInChildren<enemyAttack>().dmg = damage;
    }

    protected override void Start()
    {
        base.Start();
        anim = transform.Find("body").GetComponent<Animator>();
    }

    protected override void Update () {
        base.Update();

        //stops movement to talk to player
        if (agent.isActiveAndEnabled)
        {
            if (anim != null)
            {
                if (agent.velocity != Vector3.zero)
                    anim.SetBool("movement", true);
                else
                    anim.SetBool("movement", false);
            }

            if (gameObject.GetComponent<dialogue_npc>() != null && Distance(player.transform.position, transform.position) < 2f && !hostile)
                agent.isStopped = true;
            else
                agent.isStopped = false;
        }

        if (hostile)
        {
            if (gameObject.GetComponent<interactable>() != null)
            {
                gameObject.GetComponent<interactable>().HideE();
                Destroy(gameObject.GetComponent<interactable>());
            }

            switch (action)
            {
                case Action.follow:
                    agent.speed = 4f;
                    float dist = Distance(player.transform.position, agent.transform.position);

                    if (dist <= 2.7f)
                        action = Action.chooseAction;

                    break;

                case Action.chooseAction:
                    int atkNum = Random.Range(1, 3);
                    agent.speed = 0f;
                    if (atkNum == 1)
                        action = Action.basicAttack;
                    else
                        action = Action.specialAttack;

                    break;

                case Action.basicAttack:
                    StartCoroutine(Attack(1f, 1));
                    action = Action.follow;
                    break;

                case Action.specialAttack:
                    StartCoroutine(Attack(1f, 2));
                    action = Action.follow;
                    break;
            }
        }
    }

    public void TurnHostile()
    {
        hostile = true;
        NPC thisNpc = gameControl.control.npcs.FirstOrDefault(n => n.name == name);
        thisNpc.isHostile = true;
    }

    IEnumerator Attack(float cooldown, int atk)
    {
        float time = 0.5f;
        if (swordActive)
            yield break;
        swordActive = true;
        anim.SetTrigger("attack");
        //agent.speed = 0f;
        if (atk == 1)
        {
            damage = 2;
            sword.GetComponentInChildren<enemyAttack>().dmg = damage;
            Vector3 axis = Vector3.up;
            float angle = 90f;
            GameObject sw;
            sw = Instantiate(sword, transform.position + transform.forward, transform.rotation);
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
            yield return new WaitForSeconds(cooldown);
            swordActive = false;
            yield return null;
        } else
        {
            damage = 3;
            sword.GetComponentInChildren<enemyAttack>().dmg = damage;
            GameObject sw;
            sw = Instantiate(sword, transform.position + transform.forward, transform.rotation);
            sw.transform.parent = transform;
            float from = transform.eulerAngles.y;
            float to = from + 360f;
            float t = 0f;
            while (t < time)
            {
                t += Time.deltaTime;
                float yRot = Mathf.Lerp(from, to, t / time) % 360f;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRot, transform.eulerAngles.z);
                yield return null;
            }
            Destroy(sw);
            yield return new WaitForSeconds(cooldown);
            swordActive = false;
            yield return null;
        }
    }

    public enum Action
    {
        follow,
        chooseAction,
        basicAttack,
        specialAttack
    }
}
