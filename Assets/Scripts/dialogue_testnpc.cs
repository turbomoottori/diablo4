using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class dialogue_testnpc : MonoBehaviour {

    public TextAsset dialogue;
    char[] separators = { '|', '-', '>', '<'};
    int dl, cur, max;
    string txt;
    string[] allValues;
    GameObject control;

    void Start () {
        dl = 0;
        cur = 0;
        txt = dialogue.text;
        allValues = txt.Split(separators[1]);
        control = GameObject.Find("GameObject");
	}

    void CheckDialogue()
    {
        string[] e = allValues[dl].Split(separators[0], separators[2]);
        max = e.Length;
        if (cur == 0)
            control.GetComponent<dialogue_test>().OpenDialogue();

        if (cur < max)
        {
            if (e[cur].StartsWith("npc_"))
            {
                print("npc is talking");
                dialogue_test.talker = 0;
                cur += 1;
                CheckDialogue();
            }
            else if (e[cur].StartsWith("pl_"))
            {
                print("player is talking");
                dialogue_test.talker = 1;
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
                if (dialogue_test.talker == 1)
                {
                    string[] choices = e[cur].Split('<');
                    dialogue_test.ans = choices;
                }
                else
                {
                    dialogue_test.nextText = e[cur];
                    print(e[cur]);
                }
                cur += 1;
            }
            control.GetComponent<dialogue_test>().ChangeText();
        }
        else
        {
            control.GetComponent<dialogue_test>().CloseDialogue();
            if (dl < allValues.Length - 1)
                dl += 1;

            cur = 0;
            print("max pages reached");
        }
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
            CheckDialogue();
	}

    public static void Happening(int consequence)
    {
        switch (consequence)
        {
            case 0:
                print("0 invoked");
                break;
            case 1:
                print("1 invoked");
                break;
            case 2:
                print("2 invoked");
                break;
            case 3:
                print("3 invoked");
                break;
        }
    }
}
