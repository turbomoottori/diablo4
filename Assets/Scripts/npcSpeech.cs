using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class npcSpeech : MonoBehaviour {

    GameObject e;
    public bool wantsToTalk = true;
    bool isClose = false;
    public bool cycleTexts;
    public int dialogueNumber = 1;
    int maxDialogue;
    bool prevent = false;

    public Speak[] talks;
    public Speak[] secondDialogue;
    public Speak[] thirdDialogue;
    public Speak[] fourthDialogue;

    void Start () {
        e = Instantiate(Resources.Load("ui/interact", typeof(GameObject))) as GameObject;
        e.transform.SetParent(GameObject.Find("Canvas").transform, false);
        e.SetActive(false);

        if (secondDialogue.Length == 0)
            maxDialogue = 1;
        else if (thirdDialogue.Length == 0)
            maxDialogue = 2;
        else if (fourthDialogue.Length == 0)
            maxDialogue = 3;
        else
            maxDialogue = 4;

        print(maxDialogue);
    }
	
	void Update () {
        e.transform.position = Camera.main.WorldToScreenPoint(transform.position);

        if (wantsToTalk && isClose)
        {

            if (Input.GetKeyDown(KeyCode.E) && !menus.anyOpen)
            {
                if (gameObject.GetComponent<fetchQuest>() != null && !prevent)
                {
                    if (quests.CheckIfCompleted(gameObject.GetComponent<fetchQuest>().questToStart.questName))
                    {
                        prevent = true;
                        dialogueNumber += 1;
                        quests.QuestCompleted(gameObject.GetComponent<fetchQuest>().questToStart.questName);
                    }
                }

                if (dialogueNumber == 1)
                    menus.tempSpeak = talks;
                else if (dialogueNumber == 2)
                    menus.tempSpeak = secondDialogue;
                else if (dialogueNumber == 3)
                    menus.tempSpeak = thirdDialogue;
                else if (dialogueNumber == 4)
                    menus.tempSpeak = fourthDialogue;

                menus.talkReady = true;

                if (cycleTexts)
                {
                    dialogueNumber += 1;

                    if (dialogueNumber > maxDialogue)
                        dialogueNumber = 1;
                }

            }
        }
        else
        {
            e.SetActive(false);
        }
    }

    public void NextDialogue()
    {
        dialogueNumber += 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "interactzone")
        {
            e.SetActive(true);
            isClose = true;
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "interactzone")
        {
            e.SetActive(false);
            isClose = false;
        }
            
    }
}

[System.Serializable]
public class Speak
{
    public whoTalks whoTalks;
    [TextArea]
    public string npcTalk;
    [Tooltip("Make sure playerAnswers, npcReply and consequenses are of the same size, 1-3")]
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
