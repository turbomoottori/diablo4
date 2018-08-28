using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class civilian : enemy {

    GameObject sword;
    bool swordActive = false;
    public int attackNum;

    protected override void OnEnable()
    {
        base.OnEnable();
        sword = Resources.Load<GameObject>("enemysword");
    }
	
	protected override void Update () {
        base.Update();

        if (hostile)
        {
            float dist = Distance(player.transform.position, agent.transform.position);
            if (dist <= 1.6f)
            {
                StartCoroutine(Attack(1f));
            }
        }
    }

    IEnumerator Attack(float cooldown)
    {
        float time = 0.5f;
        if (swordActive)
            yield break;
        swordActive = true;

        //randomly choose between regular and spin attack
        int atk = Random.Range(1, 3);
        if (atk == 1)
        {
            Vector3 axis = Vector3.up;
            float angle = 90f;
            attackNum = 1;
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
            yield return new WaitForSeconds(cooldown);
            swordActive = false;
            yield return null;
        } else
        {
            attackNum = 2;
            GameObject sw;
            sw = Instantiate(sword, transform.position, transform.rotation);
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
}
