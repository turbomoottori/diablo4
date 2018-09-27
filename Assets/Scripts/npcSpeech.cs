using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class npcSpeech : MonoBehaviour {

    [TextArea]
    public string speaks;
    public int pages;
    GameObject player, globals, e;
    float dist;
    public bool wantsToTalk = true;
    public interactType type;

    public Speak[] talks;

    public List<Item> items;
    public List<Weapon> swords;
    public List<Gun> guns;

    void Start () {
        player = GameObject.Find("Player");
        globals = GameObject.Find("Globals");

        e = Instantiate(Resources.Load("ui/interact", typeof(GameObject))) as GameObject;
        e.transform.SetParent(GameObject.Find("Canvas").transform, false);

        if (type == interactType.merchant)
        {
            foreach (Weapon w in swords)
                items.Add(w);

            foreach (Gun g in guns)
                items.Add(g);

            foreach (Item i in menus.invItems)
                CheckItemDuplicates(i.name);
        }
    }

    //removes items player already owns
    void CheckItemDuplicates(string name)
    {
        Item temp = items.FirstOrDefault(i => i.name == name);
        if (temp != null)
            items.Remove(temp);
    }
	
	void Update () {
        e.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        dist = Distance(player.transform.position, transform.position);

        switch (type)
        {
            case interactType.talk:
                //player is close enough to interact
                if (wantsToTalk && dist < 2f)
                {
                    e.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E) && !menus.txtActive && !menus.pauseOpen && !menus.invOpen && !menus.stInvOpen && !menus.merch && !menus.talkReady)
                    {
                        globals.GetComponent<menus>().ChangeText(speaks, pages);

                        //if npc has a quest, make it active
                        if (gameObject.GetComponent<newQuest>() != null)
                            gameObject.GetComponent<newQuest>().CheckQuest();

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

                    if (Input.GetKeyDown(KeyCode.E) && !menus.txtActive && !menus.pauseOpen && !menus.invOpen && !menus.stInvOpen && !menus.merch && !menus.talkReady)
                    {
                        menus.chestClose = true;
                    }
                }
                else
                {
                    e.SetActive(false);
                }
                break;
            case interactType.merchant:
                //player is close enough to interact
                if (dist < 2f)
                {
                    globals.GetComponent<menus>().ChangeMerchantItems(items);
                    e.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E) && !menus.txtActive && !menus.pauseOpen && !menus.invOpen && !menus.stInvOpen && !menus.merch && !menus.talkReady)
                    {
                        menus.merchClose = true;
                    }
                }
                else
                {
                    e.SetActive(false);
                }
                break;
            case interactType.canAnswer:
                if(wantsToTalk && dist < 2f)
                {
                    e.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E) && !menus.txtActive && !menus.pauseOpen && !menus.invOpen && !menus.stInvOpen && !menus.merch && !menus.talkReady)
                    {
                        menus.tempSpeak = talks;

                        //if npc has a quest, make it active
                        if (gameObject.GetComponent<newQuest>() != null)
                            gameObject.GetComponent<newQuest>().CheckQuest();

                        menus.talkReady = true;
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
        chest,
        merchant,
        canAnswer
    }
}

[System.Serializable]
public class Speak
{
    public whoTalks whoTalks;
    [TextArea]
    public string npcTalk;
    [Tooltip("Make sure playerAnswers and npcReply are of the same size")]
    public Answer answer;
}

public enum whoTalks
{
    npc,
    player
}

[System.Serializable]
public class Answer
{
    public string[] playerAnswers;
    [TextArea]
    [Tooltip("Response to player choice of equal value")]
    public string[] npcReply;
    public UnityEvent[] consequences;
}
