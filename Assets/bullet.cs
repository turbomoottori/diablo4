using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour {

    public float range, maxRange;

	// Use this for initialization
	void Start () {
        range = 0;
	}
	
	// Update is called once per frame
	void Update () {
        range += Time.deltaTime;
        if (range >= maxRange)
        {
            Destroy(this.gameObject);
        }
	}

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "player")
            return;
    }*/
}
