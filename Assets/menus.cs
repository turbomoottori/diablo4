using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menus : MonoBehaviour {

    GameObject player, optionsMenu, pauseMenu, speech, speechBox;
    UnityEngine.UI.Text txt, pauseTitle, volumeVal;
    UnityEngine.UI.Image healthbar;
    float moveBox = 176f;
    int page, maxPages;
    public bool txtActive = false;

    GameObject pauseScreen;
    bool pauseMenuActive, talks, optionsActive = false;
    UnityEngine.UI.Slider volumeSlider;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        optionsMenu = GameObject.Find("OptionsPause");
        pauseMenu = GameObject.Find("GeneralPause");
        speech = GameObject.Find("SpeechText");
        speechBox = GameObject.Find("SpeechBox");
        pauseScreen = GameObject.Find("PauseScreen");

        pauseTitle = GameObject.Find("PauseTitle").GetComponent<UnityEngine.UI.Text>();
        volumeSlider = GameObject.Find("VolumeControl").GetComponent<UnityEngine.UI.Slider>();
        txt = speech.GetComponent<UnityEngine.UI.Text>();
        volumeVal = GameObject.Find("VolumeValue").GetComponent<UnityEngine.UI.Text>();
        healthbar = GameObject.Find("Health").GetComponent<UnityEngine.UI.Image>();

        page = 0;
        maxPages = 1;

        optionsMenu.SetActive(false);
        speechBox.SetActive(false);
        pauseScreen.SetActive(false);
    }

    private void Update()
    {
        if (txtActive && Input.GetKeyDown(KeyCode.E))
            ScrollText();

        if (Input.GetKeyDown(KeyCode.Escape) && !talks)
        {
            if (optionsActive)
            {
                Back();
            } else
            {
                pauseMenuActive = true;
                Pause();
            }
        }

        // HEALTH
        healthbar.fillAmount = (float)savedData.hp / (float)savedData.maxhp;
        if (savedData.hp <= 0)
        {
            print("kuolee");
        }


        AudioListener.volume = volumeSlider.value / 100;
        volumeVal.text = volumeSlider.value.ToString();
    }

    public void Continue()
    {
        Pause();
    }

    //open options menu
    public void Options()
    {
        optionsActive = true;
        pauseTitle.text = "OPTIONS";
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void Back()
    {
        optionsActive = false;
        pauseTitle.text = "PAUSED";
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void SaveExit()
    {
        print("save and exit");
    }

    //change text to whatever npc is saying
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
            talks = false;
            Pause();
        } else if (page == 0) {
            speechBox.SetActive(true);
            talks = true;
            Pause();
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

    //toggle pause 
    public void Pause()
    {
        if (player.GetComponent<playerMovement>().paused == false)
        {
            player.GetComponent<playerMovement>().paused = true;

            //activate pause menu
            if (pauseMenuActive)
                pauseScreen.SetActive(true);

            Time.timeScale = 0;
        }
        else
        {
            player.GetComponent<playerMovement>().paused = false;
            if (player.GetComponent<playerMovement>().slowTime)
            {
                Time.timeScale = 0.5f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
            else
            {
                Time.timeScale = 1;
                Time.fixedDeltaTime = 0.02f;
            }

            //deactivate pause menu
            if (pauseMenuActive)
            {
                pauseScreen.SetActive(false);
                pauseMenuActive = false;
            }
        }
    }
}
