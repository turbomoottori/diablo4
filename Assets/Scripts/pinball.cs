using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pinball : MonoBehaviour {

    public state gamestate;
    float fullforce;
    float springpoint, losepoint;
    public Rigidbody ball;
    PinballKeys pinballKeys;
    GameObject spring;
    float minS, maxS, timer, t;
    float percent = 0;
    Vector3 defaultGravity;
    Vector3 gravity = new Vector3(-10, -20, 0);
    float scenetimer, scenetime;

	// Use this for initialization
	void Start () {
        fullforce = 100;
        timer = 1;
        scenetimer = 20;
        scenetime = 0;
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
        defaultGravity = Physics.gravity;
        Physics.gravity = gravity;
    }
	
	// Update is called once per frame
	void Update () {
        if (scenetime < scenetimer)
            scenetime += Time.deltaTime;
        if (scenetime >= scenetimer || Input.GetKeyDown(KeyCode.Escape))
        {
            Physics.gravity = defaultGravity;
            SceneManager.LoadScene("heroroom");
        }

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
                percent = 0;
                Vector3 temp = spring.transform.position;
                spring.transform.position = Vector3.Lerp(temp, new Vector3(temp.x ,minS, temp.z), 1f);
                if (spring.transform.position.y == minS)
                {
                    if (ball.transform.position.x < springpoint)
                        gamestate = state.game;
                }
                break;
            case state.game:
                if (ball.transform.position.x > springpoint)
                    gamestate = state.cooldown;
                if (ball.transform.position.y < losepoint)
                    gamestate = state.lost;
                break;
            case state.lost:
                NewBall();
                gamestate = state.cooldown;
                break;
            case state.cooldown:
                gamestate = state.ready;
                break;
        }
	}

    void NewBall()
    {
        Vector3 ballPosition = new Vector3(-0.48f, -4.7f, -5.2f);
        Destroy(ball.gameObject);
        ball = Instantiate(Resources.Load("ball") as GameObject).GetComponent<Rigidbody>();
        ball.gameObject.transform.position = ballPosition;
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
