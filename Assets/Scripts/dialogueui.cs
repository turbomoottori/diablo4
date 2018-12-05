using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class dialogueui : MonoBehaviour {

    GameObject dBox;
    Button[] ansBoxes;
    public static int talker = 0; // 0 is npc, 1 is player
    public static bool isOpen;
    string nextText = "";
    string[] ans; //answer options
    int[] cons; //consequenses to their respective answers

    char[] separators = { '|', '-', '>', '<' };
    public int dl, cur, max; //dl is current dialogue, cur is current page of current dl, max is all pages in current dl
    string txt; //the whole dialogue tree of current npc
    string[] allValues; //all dialogues
    NPC currentNpc;
    string id;

    void Start () {
        dBox = Instantiate(Resources.Load("ui/dialoguebox") as GameObject, GameObject.Find("Canvas").transform, false);
        ansBoxes = dBox.transform.Find("player").Find("answerCont").GetComponentsInChildren<Button>();

        for (int i =0;i<ansBoxes.Length;i++)
            ansBoxes[i].onClick.AddListener(ClickAnswer);

        dBox.SetActive(false);
    }

    //activate when initiating dialogue with new npc
    public void NewDialogue(string newText, string NpcName)
    {
        currentNpc = gameControl.control.npcs.Find(n => n.name == NpcName);
        txt = newText;
        dl = currentNpc.dialoqueState;
        cur = 0;
        allValues = txt.Split(separators[1]);
        CheckDialogue();
    }

    //checks which text to show
    public void CheckDialogue()
    {
        string[] e = allValues[dl].Split(separators[0], separators[2]);
        max = e.Length;
        if (cur == 0)
            OpenDialogue();

        if (cur < max)
        {
            if (e[cur].StartsWith("npc_"))
            {
                id = e[cur];
                talker = 0;
                cur += 1;
                CheckDialogue();
            }
            else if (e[cur].StartsWith("pl_"))
            {
                id = e[cur];
                talker = 1;
                cur += 1;
                CheckDialogue();
            }
            else if (e[cur].StartsWith("invoke_"))
            {
                int newNumber = int.Parse(e[cur].Replace("invoke_", ""));
                Happening(newNumber);
                cur += 1;
                CheckDialogue();
            }
            else
            {
                if (talker == 1)
                {
                    string[] choices = e[cur].Split('<');
                    ans = choices;
                    cons = new int[ans.Length];
                }
                else
                {
                    nextText = e[cur];
                    cur += 1;
                }
            }
            ChangeText();
        }
        else
        {
            CloseDialogue();
            cur = 0;
        }
        currentNpc.dialoqueState = dl;
    }

    public void ChangeText()
    {
        switch (talker)
        {
            //npc is talking
            case 0:
                dBox.transform.Find("player").gameObject.SetActive(false);
                dBox.transform.Find("npc").gameObject.SetActive(true);
                dBox.transform.Find("npc").Find("txt").GetComponent<Text>().text = nextText;
                break;
            //player is talking
            case 1:
                dBox.transform.Find("player").gameObject.SetActive(true);
                dBox.transform.Find("npc").gameObject.SetActive(false);
                for (int i = 0; i < ansBoxes.Length; i++)
                {
                    if (i < ans.Length)
                    {
                        if (ans[i].Contains("#invoke_"))
                        {
                            string[] temp = ans[i].Split('#');
                            ans[i] = temp[0];
                            cons[i] = int.Parse(temp[1].Replace("invoke_", ""));
                        }

                        ansBoxes[i].gameObject.transform.GetChild(0).GetComponent<Text>().text = ans[i];
                    }

                    if (i >= ans.Length)
                        ansBoxes[i].gameObject.SetActive(false);
                }
                break;
        }
    }

    void FindResponse(string to)
    {
        for (int i = 0; i < allValues.Length; i++)
            if (allValues[i].StartsWith("npc_" + to + separators[0]) || allValues[i].StartsWith("pl_" + to + separators[0]))
                dl = i;

        cur = 0;
    }

    string NpcReaction(int answerNumber)
    {
        string talker;
        if (id.StartsWith("npc_"))
            talker = "npc_";
        else
            talker = "pl_";

        return id.Replace(talker, "") + answerNumber.ToString();
    }

    void ClickAnswer()
    {
        string clickedAnswer = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        for (int i = 0; i < ans.Length; i++)
        {
            if (ans[i] == clickedAnswer)
                Happening(cons[i]);
        }
    }

    public void OpenDialogue()
    {
        if (!ui.anyOpen)
            ui.TogglePause();

        dBox.SetActive(true);
        isOpen = true;
        ui.anyOpen = true;
    }

    public void CloseDialogue()
    {
        if (ui.anyOpen)
            ui.TogglePause();

        dBox.SetActive(false);
        isOpen = false;
        ui.anyOpen = false;
    }

    //stuff that can happen because of dialogue
    public void Happening(int consequence)
    {
        switch (consequence)
        {
            //cycles to the beginning
            case 0:
                CloseDialogue();
                dl = 0;
                break;
            //next conversation
            case 1:
                string talker;
                if (id.StartsWith("npc_"))
                    talker = "npc_";
                else
                    talker = "pl_";

                int newId = int.Parse(id.Replace(talker, "").Substring(0,1)) + 1;
                print(newId);
                for(int i = 0; i < allValues.Length; i++)
                    if(allValues[i].StartsWith("npc_"+newId+separators[0])|| allValues[i].StartsWith("pl_" + newId + separators[0]))
                        dl = i;
                break;
            //finds npc's response number 1
            case 2:
                FindResponse(NpcReaction(1));
                print(NpcReaction(1));
                CheckDialogue();
                break;
            //finds npc's response number 2
            case 3:
                FindResponse(NpcReaction(2));
                print(NpcReaction(2));
                CheckDialogue();
                break;
            //finds npc's response number 3
            case 4:
                FindResponse(NpcReaction(3));
                print(NpcReaction(3));
                CheckDialogue();
                break;
            //starts assigned quest
            case 5:
                if (ui.interactableObject.GetComponent<startquest>() != null)
                    ui.interactableObject.GetComponent<startquest>().CheckQuest();
                break;
            //if quest is completed, go to next conversation
            case 6:
                if(ui.interactableObject.GetComponent<startquest>()!=null)
                {
                    bool isCompleted;
                    if (ui.interactableObject.GetComponent<startquest>().type == startquest.questType.fetch)
                        isCompleted = quests.CheckIfCompleted(ui.interactableObject.GetComponent<startquest>().fQuest.questName);
                    else if (ui.interactableObject.GetComponent<startquest>().type == startquest.questType.delivery)
                        isCompleted = quests.CheckIfCompleted(ui.interactableObject.GetComponent<startquest>().dQuest.questName);
                    else
                        isCompleted = quests.CheckIfCompleted(ui.interactableObject.GetComponent<startquest>().quest.questName);

                    if (isCompleted)
                    {
                        Happening(1);
                        cur = 0;
                        CheckDialogue();
                    }
                }
                break;
            //counts if player has enough money
            case 7:
                if (ui.interactableObject.name == "metalthingdude")
                {
                    if(gameControl.control.money>=20)
                    {
                        gameControl.control.money -= 20;
                        Happening(3);
                    }
                    else
                        Happening(2);
                }
                break;
            //become hostile
            case 8:
                ui.interactableObject.GetComponent<civilian>().TurnHostile();
                break;
            //check if a quest is started
            case 9:
                if (ui.interactableObject.name == "metalthingdude")
                {
                    Quest a = quests.questList.FirstOrDefault(x => x.questName == "Help blacksmith");
                    if (a != null && !a.completed)
                    {
                        dl += 1;
                        cur = 0;
                        CheckDialogue();
                    }
                }
                break;
            //changes blacksmith's type to merchant
            case 10:
                GameObject blacksmith = ui.interactableObject;
                Destroy(blacksmith.GetComponent<dialogue_npc>());
                ui.interactableObject.AddComponent<merchant>();
                List<Item> listToAdd = new List<Item>();
                listToAdd.Add(new Weapon() { name = "cool sword", damage = 2, id = 1, baseValue = 20, speed = 0.15f, weight = 2, stackable = false, questItem = false });
                blacksmith.GetComponent<merchant>().items = listToAdd;
                blacksmith.GetComponent<merchant>().priceMultiplier = 2;
                blacksmith.GetComponent<interactable>().type = interactable.Type.merchant;
                break;
            //starts watchmaker's minigame
            case 11:
                CloseDialogue();
                cur = 0;
                dl = 6;
                SendMessage("StartMiniGame");
                break;
            //gives dash part thing to player
            case 12:
                items.ownedItems.Add(new Item() {
                    name = "part",
                    questItem = true,
                    stackable = true
                });
                break;
        }
    }
}


