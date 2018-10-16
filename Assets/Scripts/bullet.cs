using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour {

    public float range, maxRange;
    GameObject player;
    public int dmg;

	// Use this for initialization
	void Start () {
        range = 0;
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
        range = Distance(transform.position, player.transform.position);
        if (range >= maxRange)
        {
            Destroy(this.gameObject);
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "player" && collision.gameObject.tag != "enemy" && collision.gameObject.tag != gameObject.tag)
        {
            Destroy(this.gameObject);
        }

        if (this.gameObject.tag == "enemyBullet" && collision.gameObject.tag == "player")
            Destroy(this.gameObject);
    }

    public float Distance(Vector3 st, Vector3 en)
    {
        float d = 0.0f;

        float dx = st.x - en.x;
        float dy = st.y - en.y;
        float dz = st.z - en.z;

        d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);


        return d;
    }
}
