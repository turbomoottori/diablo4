using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class dialogue_test : MonoBehaviour {

    GameObject dBox;
    Transform[] ansBoxes;
    public static int talker = 0; // 0 is npc, 1 is player
    public static string nextText = "";
    public static string[] ans;
    int[] cons;

	void Start () {
        dBox = Instantiate(Resources.Load("ui/dialoguebox") as GameObject, GameObject.Find("Canvas").transform, false);
        ansBoxes = dBox.transform.Find("player").Find("answerCont").GetComponentsInChildren<Transform>();

        //FIX THIS PLS
        for(int i =0;i<ansBoxes.Length;i++)
            ansBoxes[i].GetComponent<Button>().onClick.AddListener(ClickAnswer);

        dBox.SetActive(false);
	}
	
	void Update () {
	}

    public void ChangeText()
    {
        switch (talker)
        {
            case 0:
                dBox.transform.Find("player").gameObject.SetActive(false);
                dBox.transform.Find("npc").gameObject.SetActive(true);
                dBox.transform.Find("npc").Find("txt").GetComponent<Text>().text = nextText;
                break;
            case 1:
                dBox.transform.Find("player").gameObject.SetActive(true);
                dBox.transform.Find("npc").gameObject.SetActive(false);
                for(int i = 0; i < ansBoxes.Length; i++)
                {
                    if(ans[i].Contains("#invoke_"))
                    {
                        string[] temp = ans[i].Split('#');
                        ans[i] = temp[0];
                        cons[i] = int.Parse(temp[1].Replace("invoke_", ""));
                    }

                    ansBoxes[i].GetChild(0).GetComponent<Text>().text = ans[i];

                    if (i > ans.Length)
                        ansBoxes[i].gameObject.SetActive(false);
                }
                break;
        }
    }

    public void ClickAnswer()
    {
        string clickedAnswer = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        for(int i = 0; i < ans.Length; i++)
        {
            if (ans[i] == clickedAnswer)
                dialogue_testnpc.Happening(cons[i]);
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
}


