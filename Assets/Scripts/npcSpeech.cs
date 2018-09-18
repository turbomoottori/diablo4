using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcSpeech : MonoBehaviour {

    [TextArea]
    public string speaks;
    public int pages;
    GameObject player, globals, e;
    float dist;
    public bool wantsToTalk = true;
    public interactType type;

    void Start () {
        player = GameObject.Find("Player");
        globals = GameObject.Find("Globals");

        e = Instantiate(Resources.Load("ui/interact", typeof(GameObject))) as GameObject;
        e.transform.SetParent(GameObject.Find("Canvas").transform, false);
        
    }
	
	void Update () {
        dist = Distance(player.transform.position, transform.position);
        e.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        switch (type)
        {
            case interactType.talk:
                //player is close enough to interact
                if (wantsToTalk && dist < 2f)
                {
                    e.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E) && !menus.txtActive && !menus.pauseOpen && !menus.invOpen && !menus.stInvOpen)
                    {
                        globals.GetComponent<menus>().ChangeText(speaks, pages);
                        menus.txtActive = true;
                    }
                }
                else
                {
                    e.SetActive(false);
                }
                break;
            case interactType.chest:
                //player is close enough to interact
                if (dist < 2f)
                {
                    e.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E) && !menus.txtActive && !menus.pauseOpen && !menus.invOpen && !menus.stInvOpen)
                    {
                        menus.chestClose = true;
                    }
                }
                else
                {
                    e.SetActive(false);
                }
                break;
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

    public enum interactType
    {
        talk,
        chest
    }
}
