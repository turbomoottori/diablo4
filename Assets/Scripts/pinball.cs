using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pinball : MonoBehaviour {

    public state gamestate;
    float fullforce;
    float springpoint, losepoint;
    Rigidbody ball, l, r;
    PinballKeys pinballKeys;
    GameObject spring;
    float minS, maxS, timer, t;
    float percent = 0;

	// Use this for initialization
	void Start () {
        fullforce = 200;
        timer = 1;
        ball = GetComponent<Rigidbody>();
        l = GameObject.Find("left").GetComponent<Rigidbody>();
        r = GameObject.Find("right").GetComponent<Rigidbody>();
        spring = GameObject.Find("Spring");
        minS = spring.transform.position.y;
        maxS = minS - 2f;
        losepoint = -7.26f;

        pinballKeys = new PinballKeys()
        {
            l = KeyCode.A,
            r = KeyCode.D,
            lbump = KeyCode.Q,
            rbump = KeyCode.E,
            spring = KeyCode.LeftShift
        };
    }
	
	// Update is called once per frame
	void Update () {

        switch (gamestate)
        {
            case state.ready:
                if (Input.GetKey(pinballKeys.spring) && percent < 1)
                {
                    percent += 0.5f * Time.deltaTime;
                    Vector3 curr = spring.transform.position;
                    curr.y = percent * (maxS - minS) + minS;
                    spring.transform.position = curr;
                }
                if (Input.GetKeyUp(pinballKeys.spring))
                    gamestate = state.release;
                break;
            case state.release:
                t = 0;
                ball.AddForce(Vector3.up * (fullforce * percent));
                gamestate = state.shoot;
                break;
            case state.shoot:
                Vector3 temp = spring.transform.position;
                spring.transform.position = Vector3.Lerp(temp, new Vector3(temp.x ,minS, temp.z), 1f);
                if (spring.transform.position.y == minS)
                    gamestate = state.game;
                break;
            case state.game:
                if (transform.position.x > springpoint)
                    gamestate = state.cooldown;
                if (transform.position.y < losepoint)
                    gamestate = state.lost;
                break;
            case state.lost:
                break;
            case state.cooldown:
                break;
        }
	}

    public enum state
    {
        ready,
        release,
        shoot,
        game,
        lost,
        cooldown
    }
}
