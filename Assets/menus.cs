using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class menus : MonoBehaviour {

    GameObject player, speechBox, speech, inv, itemCont;
    Text txt;
    Image healthbar;
    float moveBox = 176f;
    int page, maxPages;
    public bool txtActive = false;

    bool talks = false;
    int temphp, tempmaxhp, volumeVal;

    GameObject p, gen, opt, saves, savePrompt;
    string saveText = "Save File ";

    Transform canv;
    int lastPressed = 0;

    List<GameObject> items, storedItems;
    public List<Item> invItems;
    public static string equipOne, equipTwo;
    

    void Start () {
        player = GameObject.Find("Player");
        canv = GameObject.Find("Canvas").transform;
        healthbar = Instantiate(Resources.Load("ui/healthBar") as GameObject, canv).GetComponent<Image>();
        speechBox = Instantiate(Resources.Load("ui/speechBox") as GameObject, canv);
        speech = speechBox.transform.GetChild(0).GetChild(0).gameObject;
        txt = speech.GetComponent<Text>();
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
            TogglePause(false);

        if (Input.GetKeyDown(KeyCode.I) && !talks && !playerMovement.paused)
            Inventory();

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
    void TogglePause(bool talkPause)
    {
        //unpauses game
        if (playerMovement.paused)
        {
            //checks if player was on pause menu or talking
            if (!talkPause)
            {
                if (opt != null && opt.activeInHierarchy)
                {
                    //close options menu
                    opt.SetActive(false);
                    gen.SetActive(true);
                    gameControl.control.volume = volumeVal;
                    gameControl.control.SaveOptions();
                }
                else if (saves != null && saves.activeInHierarchy)
                {
                    //close save menu
                    saves.SetActive(false);
                    gen.SetActive(true);
                    if (savePrompt != null)
                        savePrompt.SetActive(false);
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
            else
            {
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
        //pauses game
        else if (!playerMovement.paused)
        {
            //checks if player is on pause menu or talking
            if (!talkPause)
            {
                playerMovement.paused = true;
                //pause game
                if (p == null)
                {
                    //creates pause menu
                    p = Instantiate(Resources.Load("ui/menucont") as GameObject);
                    p.transform.SetParent(canv, false);

                    gen = Instantiate(Resources.Load("ui/pause") as GameObject);
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
            } else
            {
                playerMovement.paused = true;
                Time.timeScale = 0;
            }
        }
    }

    //creates new button
    void NewButton(string name, GameObject group)
    {
        GameObject btn;
        if (name == "Back")
            btn = Instantiate(Resources.Load("ui/backButton") as GameObject, group.transform);
        else 
            btn = Instantiate(Resources.Load("ui/button") as GameObject, group.transform);
        btn.gameObject.transform.GetChild(0).GetComponent<Text>().text = name;
        btn.name = name;
        btn.GetComponent<Button>().onClick.AddListener(Click);
    }

    //creates new slider
    void NewSlider(string name, int minValue, int maxValue, int value, GameObject group)
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

    public void Click()
    {
        string btnName = EventSystem.current.currentSelectedGameObject.name;

        //CONTINUE
        if (btnName == "Continue")
        {
            TogglePause(false);
            lastPressed = 0;
        }
        //OPTIONS
        else if (btnName == "Options")
        {
            //create options menu if it doesn't exist
            if (opt == null)
            {
                opt = Instantiate(Resources.Load("ui/pause") as GameObject);
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
            lastPressed = 0;
        }
        //SAVE AND EXIT
        else if (btnName == "Save & Exit")
        {
            //opens save menu
            if (saves == null)
            {
                saves = Instantiate(Resources.Load("ui/pause") as GameObject, p.transform, false);
                NewButton(saveText + "1", saves);
                NewButton(saveText + "2", saves);
                NewButton(saveText + "3", saves);
                NewButton("Back", saves);
            }
            else if(saves != null)
            {
                saves.SetActive(true);
            }

            CheckSaves();
            lastPressed = 0;
        }
        //BACK
        else if (btnName == "Back")
        {
            TogglePause(false);
            lastPressed = 0;
        }
        //SAVE FILES
        else if(btnName == saveText + "1")
        {
            //save file 1

            //first click
            if (lastPressed != 1)
            {
                //if file already exists, show prompt--
                if (File.Exists(Application.persistentDataPath + "/save1.dat"))
                {
                    //creates prompt is it doesn't exist yet
                    if (savePrompt == null)
                        savePrompt = Instantiate(Resources.Load("ui/caution") as GameObject, saves.transform);
                    else
                        savePrompt.SetActive(true);
                }
                //--otherwise just save
                else
                {
                    gameControl.control.SaveGame(1);
                    print("exit game");
                }
                lastPressed = 1;
            }
            //second click
            else
            {
                gameControl.control.SaveGame(1);
                print("exit game");

            }
        } else if (btnName == saveText + "2")
        {
            //save file 2

            //first click
            if (lastPressed != 2)
            {
                //if file already exists, show prompt--
                if (File.Exists(Application.persistentDataPath + "/save2.dat"))
                {
                    //creates prompt is it doesn't exist yet
                    if (savePrompt == null)
                        savePrompt = Instantiate(Resources.Load("ui/caution") as GameObject, saves.transform);
                    else
                        savePrompt.SetActive(true);
                }
                //--otherwise just save
                else
                {
                    gameControl.control.SaveGame(2);
                    print("exit game");
                }
                lastPressed = 2;
            }
            //second click
            else
            {
                gameControl.control.SaveGame(2);
                print("exit game");
            }
        } else if (btnName == saveText + "3")
        {
            //save file 3

            //first click
            if (lastPressed != 3)
            {
                //if file already exists, show prompt--
                if (File.Exists(Application.persistentDataPath + "/save3.dat"))
                {
                    //creates prompt is it doesn't exist yet
                    if (savePrompt == null)
                        savePrompt = Instantiate(Resources.Load("ui/caution") as GameObject, saves.transform);
                    else
                        savePrompt.SetActive(true);
                }
                //--otherwise just save
                else
                {
                    gameControl.control.SaveGame(3);
                    print("exit game");
                }
                lastPressed = 3;
            }
            //second click
            else
            {
                gameControl.control.SaveGame(3);
                print("exit game");
            }
        }
    }

    //changes button color if save file already exists
    public void CheckSaves()
    {
        if (File.Exists(Application.persistentDataPath + "/save1.dat"))
            saves.transform.Find(saveText + "1").GetComponent<Image>().color = Color.gray;
        if (File.Exists(Application.persistentDataPath + "/save2.dat"))
            saves.transform.Find(saveText + "2").GetComponent<Image>().color = Color.gray;
        if (File.Exists(Application.persistentDataPath + "/save3.dat"))
            saves.transform.Find(saveText + "3").GetComponent<Image>().color = Color.gray;
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

    public void Inventory()
    {
        if (inv == null)
        {
            inv = Instantiate(Resources.Load("ui/inventory/inventory") as GameObject, canv, false);
            itemCont = Instantiate(Resources.Load("ui/inventory/itemInventory") as GameObject, inv.transform.Find("items").transform, false);
            if (invItems != null)
            {
                foreach (Item item in invItems)
                {
                    AddItem(item.name, item.weight);
                }
            }
            AddEquipped(equipOne);
            AddEquipped(equipTwo);
        } else
        {
            inv.SetActive(true);
        }

        foreach(Item itemInInventory in invItems)
        {
            if (!invItems.Contains(itemInInventory))
            {
                invItems.Add(itemInInventory);
            }
        }

        invItems.RemoveAll(Item => Item == null);
    }

    void AddItem(string name, int wt)
    {
        GameObject item = Instantiate(Resources.Load("ui/inventory/item") as GameObject, inv.transform, false);
        item.transform.Find("name").GetComponent<Text>().text = name;
        item.transform.Find("weight").GetComponent<Text>().text = wt.ToString();
        items.Add(item);
    }

    void AddEquipped(string name)
    {
        GameObject equip = Instantiate(Resources.Load("ui/inventory/equipped") as GameObject, inv.transform.Find("active").transform, false);
        equip.transform.Find("title").GetComponent<Text>().text = name;
        //equip.transform.Find("slot").GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/inventory/sprites/" + name);
    }

    public void InventoryClick(bool leftClick)
    {
        if (leftClick)
        {
            //assign weapon 1
        } else
        {
            //assign weapon 2
        }
    }

    void RemoveItem(string name)
    {
        foreach(GameObject item in items)
        {
            if (item.name == name)
            {
                items.Remove(item);
                storedItems.Add(item);
            }
        }
    }

    //change text to whatever npc is saying
    public void ChangeText(string NPCtext, int pages)
    {
        txt.text = NPCtext;
        maxPages = pages;
    }

    //speech box stuff
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
            TogglePause(true);
        } else if (page == 0) {
            speechBox.SetActive(true);
            talks = true;
            TogglePause(true);
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

public class Item
{
    public string name;
    public int value, weight;
}

public class Weapon : Item
{
    public int damage;
    public float speed;
}

public class Gun : Weapon
{
    public int bullets;
    public float range;
}