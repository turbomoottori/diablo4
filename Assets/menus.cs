using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class menus : MonoBehaviour {

    GameObject player, speech, speechBox;
    Text txt;
    Image healthbar;
    float moveBox = 176f;
    int page, maxPages;
    public bool txtActive = false;

    bool pauseMenuActive, talks = false;

    int temphp, tempmaxhp, volumeVal;

    GameObject p, gen, opt, saves;
    string saveText = "Save File ";

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        speech = GameObject.Find("SpeechText");
        speechBox = GameObject.Find("SpeechBox");
        txt = speech.GetComponent<Text>();
        healthbar = GameObject.Find("Health").GetComponent<Image>();

        page = 0;
        maxPages = 1;

        speechBox.SetActive(false);
        volumeVal = gameControl.control.volume;
    }

    private void Update()
    {
        if (txtActive && Input.GetKeyDown(KeyCode.E))
            ScrollText();

        //PAUSE
        if (Input.GetKeyDown(KeyCode.Escape) && !talks)
            TogglePause();

        // HEALTH
        temphp = gameControl.control.hp;
        tempmaxhp = gameControl.control.maxhp;
        healthbar.fillAmount = (float)temphp / (float)tempmaxhp;

        if (gameControl.control.hp <= 0)
        {
            print("kuolee");
        }
    }

    //PAUSE
    void TogglePause()
    {
        if (playerMovement.paused)
        {
            if (opt != null && opt.activeInHierarchy)
            {
                //close options menu
                opt.SetActive(false);
                gen.SetActive(true);
                gameControl.control.volume = volumeVal;
                gameControl.control.SaveOptions();
            } else if(saves != null && saves.activeInHierarchy)
            {
                //close save menu
                saves.SetActive(false);
                gen.SetActive(true);
            }
            else
            {
                //exit pause
                p.SetActive(false);
                playerMovement.paused = false;
                //check which timescale to use
                if (playerMovement.slowTime)
                {
                    Time.timeScale = 0.5f;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                }
                else
                {
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = 0.02f;
                }
            } 
        }
        else if (!playerMovement.paused)
        {
            playerMovement.paused = true;
            //pause game
            if (p == null)
            {
                //creates pause menu
                p = Instantiate(Resources.Load("menucont") as GameObject);
                p.transform.SetParent(GameObject.Find("Canvas").transform, false);

                gen = Instantiate(Resources.Load("pause") as GameObject);
                gen.transform.SetParent(p.transform, false);

                NewButton("Continue", gen);
                NewButton("Options", gen);
                NewButton("Save & Exit", gen);
            }
            else if (p != null)
            {
                //activates pause menu
                p.SetActive(true);
                gen.SetActive(true);
            }

            //pauses time
            Time.timeScale = 0;
        }
    }

    void NewButton(string name, GameObject group)
    {
        GameObject btn;
        btn = Instantiate(Resources.Load("button") as GameObject, group.transform);
        btn.gameObject.transform.GetChild(0).GetComponent<Text>().text = name;
        btn.name = name;
        btn.GetComponent<Button>().onClick.AddListener(Click);
    }

    void NewSlider(string name, int minValue, int maxValue, int value, GameObject group)
    {
        GameObject sl;
        sl = Instantiate(Resources.Load("slider") as GameObject, group.transform);
        sl.gameObject.name = name;
        Text txt, val;
        txt = sl.transform.Find("Name").GetComponent<Text>();
        val = sl.transform.Find("Value").GetComponent<Text>();
        txt.text = name;
        val.text = value.ToString();
        Slider theSlider = sl.transform.Find("Slider").GetComponent<Slider>();
        theSlider.minValue = minValue;
        theSlider.maxValue = maxValue;
        theSlider.value = value;
        theSlider.onValueChanged.AddListener(delegate { SliderChange(); });
    }

    public void Click()
    {
        string btnName = EventSystem.current.currentSelectedGameObject.name;

        //CONTINUE
        if (btnName == "Continue")
        {
            TogglePause();
        }
        //OPTIONS
        else if (btnName == "Options")
        {
            //create options menu if it doesn't exist
            if (opt == null)
            {
                opt = Instantiate(Resources.Load("pause") as GameObject);
                opt.transform.SetParent(p.transform, false);

                NewSlider("Volume", 0, 100, volumeVal, opt);
                NewButton("Back", opt);
            }
            else if (opt != null)
            {
                opt.SetActive(true);
            }

            //hide previous menu
            gen.SetActive(false);
        }
        //SAVE AND EXIT
        else if (btnName == "Save & Exit")
        {
            //opens save menu
            if (saves == null)
            {
                saves = Instantiate(Resources.Load("pause") as GameObject, p.transform, false);
                NewButton(saveText + "1", saves);
                NewButton(saveText + "2", saves);
                NewButton(saveText + "3", saves);
                NewButton("Back", saves);
            }
            else if(saves != null)
            {
                saves.SetActive(true);
            }
        }
        //BACK
        else if (btnName == "Back")
        {
            TogglePause();
        }
        //SAVE FILES
        else if(btnName == saveText + "1")
        {
            //save file 1
            if(File.Exists(Application.persistentDataPath + "/save1.dat"))
            {
                print("1 on jo");
            }
            else
            {
                gameControl.control.SaveGame(1);
            }
        } else if (btnName == saveText + "2")
        {
            //save file 2
            if (File.Exists(Application.persistentDataPath + "/save2.dat"))
            {
                print("2 on jo");
            }
            else
            {
                gameControl.control.SaveGame(2);
            }
        } else if (btnName == saveText + "3")
        {
            //save file 3
            if (File.Exists(Application.persistentDataPath + "/save3.dat"))
            {
                print("3 on jo");
            }
            else
            {
                gameControl.control.SaveGame(3);
            }
        }
    }

    public void SliderChange()
    {
        //checks which slider is changed
        if (EventSystem.current.currentSelectedGameObject.transform.parent.name == "Volume")
        {
            volumeVal = (int)EventSystem.current.currentSelectedGameObject.GetComponent<Slider>().value;
            EventSystem.current.currentSelectedGameObject.transform.parent.Find("Value").GetComponent<Text>().text = volumeVal.ToString();
        }

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
            TogglePause();
        } else if (page == 0) {
            speechBox.SetActive(true);
            talks = true;
            TogglePause();
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
