using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipper : MonoBehaviour {

    public leftRight lr;
    JointSpring s;
    HingeJoint hj;
    float restAngle, pressedAngle;

	// Use this for initialization
	void Start () {
        hj = GetComponent<HingeJoint>();
        restAngle = 0;
        pressedAngle = -45f;
        s.spring = 10000;
        s.damper = 150;
        GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
        GetComponent<Rigidbody>().inertiaTensor = Vector3.one;
    }
	
	// Update is called once per frame
	void Update () {
        switch (lr)
        {
            case leftRight.left:
                if (Input.GetKey(KeyCode.A))
                    s.targetPosition = pressedAngle;
                else
                    s.targetPosition = restAngle;
                break;
            case leftRight.right:
                if (Input.GetKey(KeyCode.D))
                    s.targetPosition = pressedAngle;
                else
                    s.targetPosition = restAngle;
                break;
        }
        hj.spring = s;
	}

    public enum leftRight
    {
        left,
        right
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "ball")
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        }
    }
}
