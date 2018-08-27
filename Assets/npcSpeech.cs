using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcSpeech : MonoBehaviour {

    public string speaks;
    public int pages;
    GameObject player, globals;
    float dist;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        globals = GameObject.Find("Globals");
	}
	
	// Update is called once per frame
	void Update () {
        dist = Distance(player.transform.position, transform.position);

        //player is close enough to interact
        if (dist < 2f)
        {
            print("can interact");
            if (Input.GetKeyDown(KeyCode.E) && globals.GetComponent<menus>().txtActive == false)
            {
                globals.GetComponent<menus>().ChangeText(speaks, pages);
                globals.GetComponent<menus>().txtActive = true;
            }
        }
	}

    float Distance(Vector3 st, Vector3 en)
    {
        float d = 0.0f;

        float dx = st.x - en.x;
        float dy = st.y - en.y;
        float dz = st.z - en.z;

        d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);


        return d;
    }
}
