using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class dialogue_npc : MonoBehaviour {

    public TextAsset dialogue;
    GameObject control;
    int dialogueNumber;
    public bool enemy, quest;
    bool questCompleted;

    private void Start()
    {
        control = GameObject.Find("Globals");
        if (quest)
        {
            if (GetComponent<fetchQuest>() != null)
                questCompleted = GetComponent<fetchQuest>().questToStart.completed;
            else if (GetComponent<deliveryQuest>() != null)
                questCompleted = GetComponent<deliveryQuest>().questToStart.completed;
        }

        NPC thisNpc = gameControl.control.npcs.FirstOrDefault(n => n.name == name);
        if (thisNpc != null)
            dialogueNumber = thisNpc.dialoqueState;
        else
        {
            gameControl.control.npcs.Add(new NPC() {
                name = name,
                dialoqueState = dialogueNumber,
                canBeEnemy = enemy,
                enemyKilled = false,
                hasQuest = quest,
                questCompleted=questCompleted
            });
        }
    }

    public void SendText()
    {
        control.GetComponent<dialogueui>().NewDialogue(dialogue.text, name);
    }
}
