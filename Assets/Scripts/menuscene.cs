using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class menuscene : MonoBehaviour {

    GameObject cont, main, load, options, exit;
    int volumeVal;
    string[] savedGames = new string[] {
        "Saved game 1",
        "Saved game 2",
        "Saved game 3" };

    void Start () {
        cont = GameObject.Find("buttonCont");
        main = Instantiate(Resources.Load("ui/mainmenu/container") as GameObject, cont.transform, false);
        CreateButton("New game", main);
        CreateButton("Load game", main);
        CreateButton("Options", main);
        CreateButton("Exit game", main);
        volumeVal = gameControl.control.volume;
	}

    //creates new text
    void CreateText(string text, GameObject group)
    {
        GameObject txt = Instantiate(Resources.Load("ui/mainmenu/menutext") as GameObject, group.transform, false);
        txt.gameObject.transform.GetChild(0).GetComponent<Text>().text = text;
    }

    //creates new button
    void CreateButton(string name, GameObject group)
    {
        GameObject btn;
        if (name == "Back")
            btn = Instantiate(Resources.Load("ui/mainmenu/menuback") as GameObject, group.transform);
        else
            btn = Instantiate(Resources.Load("ui/mainmenu/menubutton") as GameObject, group.transform);
        btn.gameObject.transform.GetChild(0).GetComponent<Text>().text = name;
        btn.name = name;
        btn.GetComponent<Button>().onClick.AddListener(Click);
    }

    //creates new slider
    void CreateSlider(string name, int minValue, int maxValue, int value, GameObject group)
    {
        GameObject sl;
        sl = Instantiate(Resources.Load("ui/slider") as GameObject, group.transform);
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

    //check which button is clicked
    public void Click()
    {
        string btnName = EventSystem.current.currentSelectedGameObject.name;

        if(btnName == "New game")
            StartNewGame();

        else if (btnName == "Load game")
            LoadGameScreen(true);

        else if (btnName == "Options")
            OptionsScreen(true);

        else if (btnName == "Exit game")
            ExitScreen(true);

        else if (btnName == savedGames[0])
        {
            if (File.Exists(Application.persistentDataPath + "/save1.dat"))
                LoadGame(1);
            else
                return;
        }
        else if (btnName == savedGames[1])
        {
            if (File.Exists(Application.persistentDataPath + "/save2.dat"))
                LoadGame(2);
            else
                return;
        }
        else if (btnName == savedGames[2])
        {
            if (File.Exists(Application.persistentDataPath + "/save3.dat"))
                LoadGame(3);
            else
                return;
        }

        else if (btnName == "Back")
        {
            if(load!=null && load.activeInHierarchy)
                LoadGameScreen(false);

            else if(options!=null && options.activeInHierarchy)
                OptionsScreen(false);
        }
        else if (btnName == "Yes")
        {
            if (exit != null && exit.activeInHierarchy)
                Application.Quit();
        }
        else if (btnName == "No")
        {
            if(exit != null && exit.activeInHierarchy)
                ExitScreen(false);
        }
    }

    //when slider value is changed
    public void SliderChange()
    {
        //checks which slider is changed
        if (EventSystem.current.currentSelectedGameObject.transform.parent.name == "Volume")
        {
            volumeVal = (int)EventSystem.current.currentSelectedGameObject.GetComponent<Slider>().value;
            EventSystem.current.currentSelectedGameObject.transform.parent.Find("Value").GetComponent<Text>().text = volumeVal.ToString();
        }
    }

    //starts a new game
    void StartNewGame()
    {
        gameControl.control.hp = 50;
        gameControl.control.maxhp = 50;
        gameControl.control.money = 0;
        gameControl.control.knowsDash = false;
        gameControl.control.knowsDoubleJump = false;
        gameControl.control.knowsSlowTime = false;

        if (items.storedItems == null)
            items.storedItems = new List<Item>();

        items.storedItems.Add(new Weapon() {
            name = "Sword",
            id = 0,
            baseValue = 20,
            damage = 1,
            questItem = false,
            speed = 0.2f,
            stackable = false,
            weight = 1 });

        SceneManager.LoadScene(1);
    }

    void LoadGame(int save)
    {
        gameControl.control.LoadGame(save);
    }

    //shows saved games
    void LoadGameScreen(bool open)
    {
        if (open)
        {
            if (load == null)
            {
                load = Instantiate(Resources.Load("ui/mainmenu/container") as GameObject, cont.transform, false);
                CreateButton(savedGames[0], load);
                CreateButton(savedGames[1], load);
                CreateButton(savedGames[2], load);
                CreateButton("Back", load);
            }
            else
            {
                load.SetActive(true);
            }

            main.SetActive(false);
            CheckSaves();
        }
        else
        {
            load.SetActive(false);
            main.SetActive(true);
        }
    }

    //updates saves
    void CheckSaves()
    {
        if(!File.Exists(Application.persistentDataPath + "/save1.dat"))
            load.transform.Find(savedGames[0]).GetChild(0).GetComponent<Text>().text = "Empty";
        else
            load.transform.Find(savedGames[0]).GetChild(0).GetComponent<Text>().text = savedGames[0];

        if (!File.Exists(Application.persistentDataPath + "/save2.dat"))
            load.transform.Find(savedGames[1]).GetChild(0).GetComponent<Text>().text = "Empty";
        else
            load.transform.Find(savedGames[1]).GetChild(0).GetComponent<Text>().text = savedGames[1];

        if (!File.Exists(Application.persistentDataPath + "/save3.dat"))
            load.transform.Find(savedGames[2]).GetChild(0).GetComponent<Text>().text = "Empty";
        else
            load.transform.Find(savedGames[2]).GetChild(0).GetComponent<Text>().text = savedGames[2];
    }

    //opens options
    void OptionsScreen(bool open)
    {
        if (open)
        {
            if (options == null)
            {
                options = Instantiate(Resources.Load("ui/mainmenu/container") as GameObject, cont.transform, false);
                CreateSlider("Volume", 0, 100, volumeVal, options);
                CreateButton("Back", options);
            }
            else
            {
                options.SetActive(true);
            }

            main.SetActive(false);
        }
        else
        {
            //saves options
            gameControl.control.volume = volumeVal;
            gameControl.control.SaveOptions();
            options.SetActive(false);
            main.SetActive(true);
        }
    }

    //opens exit prompt
    void ExitScreen(bool open)
    {
        if (open)
        {
            if (exit == null)
            {
                exit = Instantiate(Resources.Load("ui/mainmenu/container") as GameObject, cont.transform, false);
                CreateText("Are you sure you want to exit", exit);
                CreateButton("Yes", exit);
                CreateButton("No", exit);
            }
            else
            {
                exit.SetActive(true);
            }

            main.SetActive(false);
        }
        else
        {
            exit.SetActive(false);
            main.SetActive(true);
        }
    }
}
