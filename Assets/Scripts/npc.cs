using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class npc : MonoBehaviour {

    //public Dialogue[] dialogue;
    public DialogueTree[] dialogues;
    public bool cycleText;
    public int currentDialogue;
    bool qCompleted;
	
	public void Interact()
    {
        if (GetComponent<fetchQuest>() != null)
        {
            if (qCompleted && currentDialogue == 2)
                currentDialogue = 3;

            bool questCompleted = quests.CheckIfCompleted(GetComponent<fetchQuest>().questToStart.questName);
            if (questCompleted)
            {
                quests.QuestCompleted(GetComponent<fetchQuest>().questToStart.questName);
                qCompleted = true;
                if (currentDialogue < 2)
                    currentDialogue += 1;
            }
        }

        if (cycleText)
        {
            currentDialogue += 1;
            if (currentDialogue > dialogues.Length)
                currentDialogue = 1;
        }
    }

    public void NextConvo()
    {
        if (currentDialogue < dialogues.Length)
            currentDialogue += 1;
    }
}

[System.Serializable]
public class DialogueTree
{
    public Dialogue[] dialogue;
}

[System.Serializable]
public class Dialogue
{
    public who who;
    public npcDialogue npc;
    public playerDialogue[] player;
}

[System.Serializable]
public class npcDialogue
{
    [TextArea]
    public string talk;
    public UnityEvent consequenseWithoutAction;
}

[System.Serializable]
public class playerDialogue
{
    public string answer;
    public string reactionToAnswer;
    public UnityEvent consequenceToAnswer;
}

public enum who
{
    npc,
    player
}
