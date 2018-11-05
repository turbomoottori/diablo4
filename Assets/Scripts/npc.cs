using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class npc : MonoBehaviour {

    [Tooltip("how many different dialogue scenarios there are in total")]
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

        if(GetComponent<deliveryQuest>() != null)
        {
            bool questCompleted = quests.CheckIfCompleted(GetComponent<deliveryQuest>().questToStart.questName);
            if (questCompleted && !qCompleted)
            {
                quests.QuestCompleted(GetComponent<deliveryQuest>().questToStart.questName);
                qCompleted = true;
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
    [Tooltip("how long the current conversation will take")]
    public Dialogue[] dialogue;
}

[System.Serializable]
public class Dialogue
{
    public who who;
    public npcDialogue npc;
    [Tooltip("how many answer options does the player have")]
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
    [Tooltip("npc's reaction to said answer")]
    public string reactionToAnswer;
    public UnityEvent consequenceToAnswer;
}

public enum who
{
    npc,
    player
}
