using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class dialogue_test : MonoBehaviour {

    GameObject dBox;
    Button[] ansBoxes;
    public static int talker = 0; // 0 is npc, 1 is player
    string nextText = "";
    string[] ans; //answer options
    int[] cons; //consequenses to their respective answers

    char[] separators = { '|', '-', '>', '<' };
    int dl, cur, max; //dl is current dialogue, cur is current page of current dl, max is all pages in current dl
    string txt; //the whole dialogue tree of current npc
    string[] allValues; //all dialogues
    NPC currentNpc;

    void Start () {
        dBox = Instantiate(Resources.Load("ui/dialoguebox") as GameObject, GameObject.Find("Canvas").transform, false);
        ansBoxes = dBox.transform.Find("player").Find("answerCont").GetComponentsInChildren<Button>();

        for (int i =0;i<ansBoxes.Length;i++)
            ansBoxes[i].onClick.AddListener(ClickAnswer);

        dBox.SetActive(false);
    }

    private void Update()
    {
        //for testing only
        if (Input.GetKeyDown(KeyCode.U) && talker != 1)
            CheckDialogue();
    }

    //activate when initiating dialogue with new npc
    public void NewDialogue(string newText, string NpcName)
    {
        currentNpc = gameControl.control.npcs.Find(n => n.name == NpcName);
        txt = newText;
        dl = currentNpc.dialoqueState;
        cur = 0;
        allValues = txt.Split(separators[1]);
    }

    //checks which text to show
    void CheckDialogue()
    {
        string[] e = allValues[dl].Split(separators[0], separators[2]);
        max = e.Length;

        if (cur == 0)
            OpenDialogue();

        if (cur < max)
        {
            if (e[cur].StartsWith("npc_"))
            {
                print("npc is talking");
                talker = 0;
                cur += 1;
                CheckDialogue();
            }
            else if (e[cur].StartsWith("pl_"))
            {
                print("player is talking");
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
                }
                cur += 1;
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
        dBox.SetActive(true);
    }

    public void CloseDialogue()
    {
        dBox.SetActive(false);
    }

    public void Happening(int consequence)
    {
        switch (consequence)
        {
            case 0:
                print("start quest");
                break;
            case 1:
                print("change dialogue");
                dl += 1;
                break;
            case 2:
                print("yes");
                CheckDialogue();
                break;
            case 3:
                print("no");
                CheckDialogue();
                break;
        }
    }
}


