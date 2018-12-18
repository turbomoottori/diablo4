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
        if (quest && GetComponent<startquest>() != null)
        {
            switch (GetComponent<startquest>().type)
            {
                case startquest.questType.regular:
                    questCompleted = GetComponent<startquest>().quest.completed;
                    break;
                case startquest.questType.delivery:
                    questCompleted = GetComponent<startquest>().dQuest.completed;
                    break;
                case startquest.questType.fetch:
                    questCompleted = GetComponent<startquest>().fQuest.completed;
                    break;
            }
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
