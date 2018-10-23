using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class npcSpeech : MonoBehaviour {
    /*
    public bool wantsToTalk = true;
    public bool cycleTexts;
    public int dialogueNumber = 1;
    int maxDialogue;
    bool prevent = false;

    [Tooltip("If npc only has one thing to say, use only the first dialogue")]
    public Speak[] firstDialogue;
    public Speak[] secondDialogue; 
    public Speak[] thirdDialogue;
    public Speak[] fourthDialogue;

    void Start () {
        if (secondDialogue.Length == 0)
            maxDialogue = 1;
        else if (thirdDialogue.Length == 0)
            maxDialogue = 2;
        else if (fourthDialogue.Length == 0)
            maxDialogue = 3;
        else
            maxDialogue = 4;
    }

    public void Interact()
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
            menus.tempSpeak = firstDialogue;
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

    public void NextDialogue()
    {
        dialogueNumber += 1;
    }*/
}
/*
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
    [Tooltip("Consequences to player choice of equal value")]
    public UnityEvent[] consequences;
}*/
