using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class npc : MonoBehaviour {

    [Tooltip("how many different dialogue scenarios there are in total")]
    public DialogueTree[] dialogues;
    public bool cycleText;
    public int currentDialogue;
    bool qCompleted;

    private void Start()
    {
        QuestGivers q = gameControl.control.questGivers.FirstOrDefault(a => a.name == this.name);
        if (q == null)
            gameControl.control.questGivers.Add(new QuestGivers() { name = this.name, questStage = currentDialogue, questCompleted = qCompleted });
        else if (q != null)
        {
            currentDialogue = q.questStage;
            qCompleted = q.questCompleted;
        }
    }

    public void Interact()
    {
        GetComponent<interactable>().HideE();
        if (name == "metalthingdude")
        {
            Quest a = quests.questList.FirstOrDefault(x => x.questName == "Help blacksmith");
            if (a != null && !a.completed && GetComponent<npc>().currentDialogue == 0)
                GetComponent<npc>().NextConvo();
        }

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
                print(currentDialogue);
            }
        }

        if (cycleText)
        {
            currentDialogue += 1;
            if (currentDialogue > dialogues.Length)
                currentDialogue = 1;
        }
        print(currentDialogue);
        QuestGivers q = gameControl.control.questGivers.FirstOrDefault(a => a.name == this.name);
        q.questStage = currentDialogue;
        q.questCompleted = qCompleted;
    }

    public void NextConvo()
    {
        if (currentDialogue < dialogues.Length)
            currentDialogue += 1;

        QuestGivers q = gameControl.control.questGivers.FirstOrDefault(a => a.name == this.name);
        q.questStage = currentDialogue;
    }

    public void SpecificConvoNumber(int number)
    {
        currentDialogue = number;
        QuestGivers q = gameControl.control.questGivers.FirstOrDefault(a => a.name == this.name);
        q.questStage = currentDialogue;
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
