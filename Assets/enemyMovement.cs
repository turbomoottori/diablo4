using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyMovement : MonoBehaviour {

    GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("player");
    }
	
	// Update is called once per frame
	void Update () {
        float t = 0f;
        while (t < 10)
        {
            t += Time.deltaTime;
            //subtract AI thing’s position from waypoint, player, whatever it is going towards…

            Vector3 target = player.transform.position - transform.position;

            //normalize it to get direction
            target = target.normalized;

            //now make a new raycast hit
            //and draw a line from the AI out some distance in the ‘forward direction

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 4f))
            {

                //check that its not hitting itself
                //then add the normalised hit direction to your direction plus some repulsion force -in my case // 400f

                if (hit.transform != transform)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);

                    target += hit.normal * 4f;
                }

            }

            //now make two more raycasts out to the left and right to make the cornering more accurate and reducing collisions more

            Vector3 leftR = transform.position;
            Vector3 rightR = transform.position;

            leftR.x -= 1;
            rightR.x += 1;

            if (Physics.Raycast(leftR, transform.forward, out hit, 4f))
            {
                if (hit.transform != transform)
                {
                    Debug.DrawLine(leftR, hit.point, Color.red);
                    target += hit.normal * 2f;
                }

            }
            if (Physics.Raycast(rightR, transform.forward, out hit, 4f))
            {
                if (hit.transform != transform)
                {
                    Debug.DrawLine(rightR, hit.point, Color.red);

                    target += hit.normal * 2f;
                }

            }

            // then set the look rotation toward this new target based on the collisions

            Quaternion torotation = Quaternion.LookRotation(target);
            torotation.x = 0;
            torotation.z = 0;

            //then slerp the rotation


            transform.rotation = Quaternion.Slerp(transform.rotation, torotation, Time.deltaTime * 10f);

        }


        //finally add some propulsion to move the object forward based on this rotation
        //mine is a little more complicated than below but you hopefully get the idea…

        transform.position += transform.forward * 2f * Time.deltaTime;
    }

}
