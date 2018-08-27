using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menus : MonoBehaviour {

    GameObject player, optionsMenu, pauseMenu, speech, speechBox;
    UnityEngine.UI.Text txt;
    float moveBox = 126f;
    int page, maxPages;
    public bool txtActive = false;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        optionsMenu = GameObject.Find("OptionsPause");
        pauseMenu = GameObject.Find("GeneralPause");
        speech = GameObject.Find("SpeechText");
        speechBox = GameObject.Find("SpeechBox");
        txt = speech.GetComponent<UnityEngine.UI.Text>();
        optionsMenu.SetActive(false);
        page = 0;
        maxPages = 1;
        speechBox.SetActive(false);
	}

    private void Update()
    {
        if (txtActive && Input.GetKeyDown(KeyCode.E))
            ScrollText();   
    }

    public void Continue()
    {
        player.GetComponent<playerMovement>().Pause();
    }

    public void Options()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void Back()
    {
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void SaveExit()
    {
        print("save and exit");
    }

    public void ChangeText(string NPCtext, int pages)
    {
        txt.text = NPCtext;
        maxPages = pages;
    }

    public void ScrollText()
    {
        RectTransform box = speech.GetComponent<RectTransform>();
        //close, open, scroll
        if (page >= maxPages)
        {
            speechBox.SetActive(false);
            box.anchoredPosition = Vector2.zero;
            box.offsetMin = Vector2.zero;
            txtActive = false;
            page = 0;
            player.GetComponent<playerMovement>().talks = false;
        } else if (page == 0) {
            speechBox.SetActive(true);
            player.GetComponent<playerMovement>().talks = true;
            page += 1;
        } else
        {
            Vector2 nextPos = box.anchoredPosition;
            nextPos.y += moveBox;
            box.anchoredPosition = nextPos;
            box.offsetMin = Vector2.zero;
            page += 1;
        }
    }
}
