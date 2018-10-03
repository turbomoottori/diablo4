using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class menus : MonoBehaviour
{

    GameObject speechBox, speech, playerTalk, playerChoice;
    GameObject talkBox, talkText, ansCont;
    Text txt;
    Image healthbar;
    float moveBox = 176f;
    int page, maxPages;
    public static bool txtActive, chestClose, merchClose, talkReady = false;
    public static bool anyOpen = false;
    public bool choice = false;

    bool talks = false;
    public static bool pauseOpen, invOpen, stInvOpen, merch = false;
    int temphp, tempmaxhp, volumeVal;

    GameObject p, gen, opt, saves, savePrompt;
    string saveText = "Save File ";
    public static GameObject collected;
    public static float showCollectibleTime;
    public static bool showC;

    Transform canv;
    int lastPressed = 0;

    public static Speak[] tempSpeak;
    int currentPage = 0;

    //inventory 
    GameObject inv, itemCont, stInv, stInInventory, stStored;
    GameObject merchCont, ownedItems, shopItems, money;
    public static List<Item> invItems = new List<Item>();
    public static List<Item> itemsStored = new List<Item>();
    List<Item> merchItems = new List<Item>();
    public static string equipOne, equipTwo;

    void Start()
    {
        canv = GameObject.Find("Canvas").transform;
        healthbar = Instantiate(Resources.Load("ui/healthBar") as GameObject, canv).transform.GetChild(0).GetChild(0).GetComponent<Image>();
        speechBox = Instantiate(Resources.Load("ui/speechBox") as GameObject, canv);
        speech = speechBox.transform.GetChild(0).GetChild(0).gameObject;
        txt = speech.GetComponent<Text>();

        //temp values for speech bubbles
        page = 0;
        maxPages = 1;

        speechBox.SetActive(false);
        volumeVal = gameControl.control.volume;

        print(invItems.Count);

        talkBox = Instantiate(Resources.Load("ui/speechBoxTwo") as GameObject, canv);
        talkBox.SetActive(false);
        talkText = Instantiate(Resources.Load("ui/speechText") as GameObject, talkBox.transform.GetChild(0).transform);
        ansCont = Instantiate(Resources.Load("ui/answerCont") as GameObject, talkBox.transform.GetChild(0).transform);
    }

    private void Update()
    {
        //INTERACT
        if (Input.GetKeyDown(KeyCode.E) && !pauseOpen)
        {
            //TALK
            if (txtActive)
                Talk();
            //OPEN CHEST
            else if (chestClose)
                StoredItems();
            //SHOW MERCH ITEMS
            else if (merchClose)
                MerchantUI();
            else if (talkReady && !choice)
                Talk();
        }

        //PRESSING ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!talks && !invOpen && !stInvOpen && !stInvOpen && !merch)
                TogglePause();
            else if (!pauseOpen && !talks && invOpen && !stInvOpen && !merch)
                Inventory();
            else if (!pauseOpen && !invOpen && talks && !stInvOpen && !merch && !choice)
                Talk();
            else if (!pauseOpen && !invOpen && !talks && stInvOpen && !merch)
                StoredItems();
            else if (!pauseOpen && !invOpen && !talks && !stInvOpen && merch)
                MerchantUI();
        }

        //OPEN INVENTORY
        if (Input.GetKeyDown(KeyCode.I) && !talks && !pauseOpen && !stInvOpen && !merch)
            Inventory();

        // HEALTH
        temphp = gameControl.control.hp;
        tempmaxhp = gameControl.control.maxhp;
        healthbar.fillAmount = (float)temphp / (float)tempmaxhp;

        //DIE
        if (gameControl.control.hp <= 0)
        {
            print("kuolee");
        }

        //SHOW COLLECTED ITEM   
        showCollectibleTime += Time.deltaTime;
        if (showCollectibleTime >= 4 && showC)
        {
            showC = false;
            HideCollected();
        }
    }

    //talking to npc
    public void Talk()
    {
        int talkPages = tempSpeak.Length;

        //cycles through conversation
        if (currentPage < talkPages)
        {
            if (currentPage == 0)
            {
                talkBox.SetActive(true);
                PauseNoMenu();
                talks = true;
                anyOpen = true;
            }

            if (tempSpeak[currentPage].whoTalks == whoTalks.npc)
            {
                choice = false;
                ansCont.SetActive(false);
                talkText.SetActive(true);
                talkText.GetComponent<Text>().text = tempSpeak[currentPage].npcTalk;
            }
            else
            {
                choice = true;
                ansCont.SetActive(true);
                if (ansCont.transform != null)
                    foreach (Transform child in ansCont.transform)
                        Destroy(child.gameObject);

                talkText.SetActive(false);
                for (int i = 0; i < tempSpeak[currentPage].answer.playerAnswers.Length; i++)
                {
                    GameObject ans = Instantiate(Resources.Load("ui/answerButton") as GameObject, ansCont.transform);
                    ans.GetComponent<Button>().onClick.AddListener(PlayerChoice);
                    ans.name = i.ToString();
                    ans.transform.GetChild(0).GetComponent<Text>().text = tempSpeak[currentPage].answer.playerAnswers[i];
                }
            }

            currentPage += 1;
        }
        //ends conversation
        else
        {
            talkBox.SetActive(false);
            talkReady = false;
            currentPage = 0;
            talks = false;
            anyOpen = false;
            PauseNoMenu();
        }
    }

    //making a choice
    void PlayerChoice()
    {
        string btnName = EventSystem.current.currentSelectedGameObject.name;
        int pg = currentPage - 1;

        for (int i = 0; i < tempSpeak[pg].answer.playerAnswers.Length; i++)
        {
            if (btnName == i.ToString())
            {
                ansCont.SetActive(false);
                talkText.SetActive(true);
                talkText.GetComponent<Text>().text = tempSpeak[pg].answer.npcReply[i];
                choice = false;

                if (tempSpeak[pg].answer.consequences[i] != null)
                    tempSpeak[pg].answer.consequences[i].Invoke();
            }
        }
    }

    //PAUSE
    void TogglePause()
    {
        if (playerMovement.paused)
        {
            //unpause
            pauseOpen = false;
            anyOpen = false;

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
        else if (!playerMovement.paused)
        {
            playerMovement.paused = true;
            pauseOpen = true;
            anyOpen = true;

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
        }
    }

    //pause without displaying menus
    void PauseNoMenu()
    {
        if (playerMovement.paused)
        {
            //unpause
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
        else if (!playerMovement.paused)
        {
            //pause
            playerMovement.paused = true;
            Time.timeScale = 0;
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

    //when clicked on a button
    public void Click()
    {
        string btnName = EventSystem.current.currentSelectedGameObject.name;

        //CONTINUE
        if (btnName == "Continue")
        {
            TogglePause();
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
            else if (saves != null)
            {
                saves.SetActive(true);
            }

            CheckSaves();
            lastPressed = 0;
        }
        //BACK
        else if (btnName == "Back")
        {
            TogglePause();
            lastPressed = 0;
        }
        //SAVE FILES
        else if (btnName == saveText + "1")
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
        }
        else if (btnName == saveText + "2")
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
        }
        else if (btnName == saveText + "3")
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

    //changes slider values
    public void SliderChange()
    {
        //checks which slider is changed
        if (EventSystem.current.currentSelectedGameObject.transform.parent.name == "Volume")
        {
            volumeVal = (int)EventSystem.current.currentSelectedGameObject.GetComponent<Slider>().value;
            EventSystem.current.currentSelectedGameObject.transform.parent.Find("Value").GetComponent<Text>().text = volumeVal.ToString();
        }
    }

    //toggle inventory
    public void Inventory()
    {
        if (!invOpen)
        {
            invOpen = true;
            anyOpen = true;

            PauseNoMenu();

            //create inventory menu if it doesn't exist
            if (inv == null)
            {
                inv = Instantiate(Resources.Load("ui/inventory/inventory") as GameObject, canv, false);
                itemCont = Instantiate(Resources.Load("ui/inventory/itemInventory") as GameObject, inv.transform.Find("items").transform, false);

                //show every item in inventory
                if (invItems != null)
                    foreach (Item item in invItems)
                        AddItem(item.name, item.weight, itemCont, buttonScript.buttonType.equip);

                if (equipOne == null)
                    equipOne = "Empty";
                if (equipTwo == null)
                    equipTwo = "Empty";

                //display equipped items
                AddEquipped(equipOne, 1);
                AddEquipped(equipTwo, 2);

                if (quests.questList != null)
                {
                    foreach (Quest quest in quests.questList)
                    {
                        AddQuest(quest.questName, quest.questDesc);
                    }

                    CheckQuests();
                }
            }
            else
            {
                inv.SetActive(true);
            }

            //if there's new items add them too
            foreach (Item itemInInventory in invItems)
                if (!itemCont.transform.Find(itemInInventory.name))
                {
                    AddItem(itemInInventory.name, itemInInventory.weight, itemCont, buttonScript.buttonType.equip);
                    List<Item> tempList = invItems.FindAll(i => i.name.Equals(itemInInventory.name));
                    string stacks = " (" + tempList.Count.ToString() + ")";
                }

            //checks duplicates
            CheckDuplicates(itemCont, invItems);

            foreach (Transform child in itemCont.transform)
            {
                if (child.name == equipOne || child.name == equipTwo)
                    child.GetComponent<Image>().color = Color.gray;
                else
                    child.GetComponent<Image>().color = Color.white;
            }

            if (quests.questList != null)
                CheckQuests();

            invItems.RemoveAll(Item => Item == null);

        }
        //closes inventory
        else if (invOpen)
        {
            invOpen = false;
            anyOpen = false;
            inv.SetActive(false);
            PauseNoMenu();
        }
    }

    //display items in inventory
    void AddItem(string name, int wt, GameObject place, buttonScript.buttonType type)
    {
        GameObject i = Instantiate(Resources.Load("ui/inventory/item") as GameObject, place.transform, false);
        i.transform.Find("name").GetComponent<Text>().text = name;
        i.transform.Find("weight").GetComponent<Text>().text = wt.ToString();
        i.name = name;
        i.GetComponent<buttonScript>().type = type;
    }

    //display equipped items
    void AddEquipped(string name, int equipNumber)
    {
        GameObject equip = Instantiate(Resources.Load("ui/inventory/equipped") as GameObject, inv.transform.Find("active").transform, false);
        equip.transform.Find("title").GetComponent<Text>().text = name;
        equip.name = name + equipNumber.ToString();
        //equip.transform.Find("slot").GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/inventory/sprites/" + name);
    }

    //adds quests to list
    void AddQuest(string name, string desc)
    {
        GameObject q = Instantiate(Resources.Load("ui/inventory/questblock") as GameObject, inv.transform.Find("quests").transform, false);
        q.name = "quest" + name;
        q.transform.Find("Title").GetComponent<Text>().text = name;
        q.transform.Find("Title").name = name;
        q.transform.Find("Desc").GetComponent<Text>().text = desc;
        q.transform.Find("Desc").name = name + "desc";
    }

    //checks duplicates
    void CheckDuplicates(GameObject g, List<Item> l)
    {
        foreach (Transform child in g.transform)
        {
            Item temp = l.FirstOrDefault(i => i.name == child.name);
            Transform[] duplicateCheck = FindChildren(g.transform, child.name);

            //if there's more than one item of the same name, delete the other one
            if (duplicateCheck.Length > 1)
            {
                for (int i = 0; i < duplicateCheck.Length; i++)
                    if (i != 0)
                        Destroy(duplicateCheck[i].gameObject);
            }

            //display amount if item is stackable
            if (temp != null && temp.stackable)
            {
                List<Item> tempList = l.FindAll(i => i.name.Equals(child.name));
                string stacks = " (" + tempList.Count.ToString() + ")";
                child.transform.Find("name").GetComponent<Text>().text = temp.name + stacks;
            }
            if (temp == null)
                Destroy(child.gameObject);


        }
    }

    //checks if quest is finished
    void CheckQuests()
    {
        foreach (Quest activeQuest in quests.questList)
            if (!inv.transform.Find("quests").transform.Find("quest" + activeQuest.questName))
                AddQuest(activeQuest.questName, activeQuest.questDesc);


        string completedText = "DONE";
        foreach (Quest quest in quests.questList)
        {
            if (quest.completed)
            {
                inv.transform.Find("quests").transform.Find("quest" + quest.questName).transform.Find(quest.questName + "desc").GetComponent<Text>().text = completedText;
            }
        }
    }

    //equipping weapons
    public void InventoryClick(bool leftClick, string name)
    {
        //assigning weapon 1
        if (leftClick)
        {
            foreach (Item item in invItems)
            {
                //equips only if item is a weapons
                if (item.name == name && item is Weapon)
                {
                    if (name == equipOne || name == equipTwo)
                    {
                        //this item is already equipped
                        print("already equipped");
                    }
                    else
                    {
                        if (item is Gun)
                        {
                            weapons.weaponType1 = 2;
                            //recast as gun
                            Gun g = item as Gun;
                            weapons.damage1 = g.damage;
                            weapons.speed1 = g.speed;
                            weapons.bullets1 = g.bullets;
                            weapons.range1 = g.range;
                            weapons.type1 = g.type;
                            weapons.special1 = g.special;
                            weapons.rlspeed1 = g.rlspeed;
                            print("is gun");
                        }
                        else
                        {
                            //item is a sword
                            weapons.weaponType1 = 1;
                            //recast as weapon
                            Weapon w = item as Weapon;
                            weapons.damage1 = w.damage;
                            weapons.speed1 = w.speed;
                            print("is not a gun");
                        }

                        inv.transform.Find("active").transform.Find(equipOne + "1").transform.Find("title").GetComponent<Text>().text = name;
                        equipOne = name;
                    }
                }
            }
        }
        //assigning weapon 2
        else
        {
            foreach (Item item in invItems)
            {
                if (item.name == name && item is Weapon)
                {
                    if (name == equipTwo || name == equipOne)
                    {
                        print("already equipped");
                    }
                    else
                    {
                        if (item is Gun)
                        {
                            weapons.weaponType2 = 2;
                            //recast as gun
                            Gun g = item as Gun;
                            weapons.damage2 = g.damage;
                            weapons.speed2 = g.speed;
                            weapons.bullets2 = g.bullets;
                            weapons.range2 = g.range;
                            weapons.type2 = g.type;
                            weapons.special2 = g.special;
                            weapons.rlspeed2 = g.rlspeed;
                            print("is gun");
                        }
                        else
                        {
                            weapons.weaponType2 = 1;
                            //recast as weapon
                            Weapon w = item as Weapon;
                            weapons.damage2 = w.damage;
                            weapons.speed2 = w.speed;
                            print("is not a gun");
                        }

                        inv.transform.Find("active").transform.Find(equipTwo + "2").transform.Find("title").GetComponent<Text>().text = name;
                        equipTwo = name;
                    }
                }
            }
        }

        foreach (Transform child in itemCont.transform)
        {
            if (child.name == equipOne || child.name == equipTwo)
                child.GetComponent<Image>().color = Color.gray;
            else
                child.GetComponent<Image>().color = Color.white;
        }
    }

    //click function for storage ui
    public void InventoryClickStorage(string name, bool take)
    {
        //taking from storage to inventory
        if (take)
        {
            Item tempItem = itemsStored.FirstOrDefault(i => i.name == name);
            if (tempItem != null)
            {
                itemsStored.Remove(tempItem);
                invItems.Add(tempItem);

                //adds to another list
                foreach (Item itemInInventory in invItems)
                    if (!stInInventory.transform.Find(itemInInventory.name))
                        AddItem(itemInInventory.name, itemInInventory.weight, stInInventory, buttonScript.buttonType.storable);
            }
        }
        //taking from inventory to storage
        else
        {
            if (name == equipOne || name == equipTwo)
            {
                print("equipped, cannot store");
            }
            else
            {
                Item tempItem = invItems.FirstOrDefault(i => i.name == name);
                if (tempItem != null)
                {
                    invItems.Remove(tempItem);
                    itemsStored.Add(tempItem);

                    //adds to another list
                    foreach (Item itemStored in itemsStored)
                        if (!stStored.transform.Find(itemStored.name))
                            AddItem(itemStored.name, itemStored.weight, stStored, buttonScript.buttonType.stored);
                }
            }
        }

        CheckDuplicates(stInInventory, invItems);
        CheckDuplicates(stStored, itemsStored);
    }

    //opens and closes storage ui 
    public void StoredItems()
    {
        //opens storage ui
        if (!stInvOpen)
        {
            stInvOpen = true;
            anyOpen = true;

            if (stInv == null)
            {
                stInv = Instantiate(Resources.Load("ui/inventory/storeInv") as GameObject, canv, false);
                stInInventory = Instantiate(Resources.Load("ui/inventory/itemInventory") as GameObject, stInv.transform.Find("items"), false);
                stStored = Instantiate(Resources.Load("ui/inventory/itemInventory") as GameObject, stInv.transform.Find("stored"), false);

                //show every item in inventory
                if (invItems != null)
                    foreach (Item item in invItems)
                        AddItem(item.name, item.weight, stInInventory, buttonScript.buttonType.storable);

                //show every stored item
                if (itemsStored != null)
                    foreach (Item stitem in itemsStored)
                        AddItem(stitem.name, stitem.weight, stStored, buttonScript.buttonType.stored);
            }
            else
            {
                stInv.SetActive(true);
            }

            foreach (Item itemInInventory in invItems)
                if (!stInInventory.transform.Find(itemInInventory.name))
                    AddItem(itemInInventory.name, itemInInventory.weight, stInInventory, buttonScript.buttonType.storable);

            foreach (Item itemStored in itemsStored)
                if (!stStored.transform.Find(itemStored.name))
                    AddItem(itemStored.name, itemStored.weight, stStored, buttonScript.buttonType.stored);

            //changes color of already equipped item
            foreach (Item item in invItems)
            {
                if (item.name == equipOne || item.name == equipTwo)
                {
                    stInInventory.transform.Find(item.name).GetComponent<Image>().color = Color.gray;
                }
                else
                {
                    stInInventory.transform.Find(item.name).GetComponent<Image>().color = Color.white;
                }
            }
            CheckDuplicates(stStored, itemsStored);
            CheckDuplicates(stInInventory, invItems);


            //check if item is removed and remove it from the list
            invItems.RemoveAll(Item => Item == null);
        }
        else
        {
            //close this inventory
            stInvOpen = false;
            anyOpen = false;
            stInv.SetActive(false);
            chestClose = false;
        }

        //toggles timescale
        PauseNoMenu();
    }

    public void ChangeMerchantItems(List<Item> items)
    {
        merchItems = items;
    }

    //opens and closes storage ui 
    public void MerchantUI()
    {
        //opens storage ui
        if (!merch)
        {
            merch = true;
            anyOpen = true;

            if (merchCont == null)
            {
                merchCont = Instantiate(Resources.Load("ui/inventory/merchantUI") as GameObject, canv, false);
                ownedItems = Instantiate(Resources.Load("ui/inventory/itemInventory") as GameObject, merchCont.transform.Find("items"), false);
                shopItems = Instantiate(Resources.Load("ui/inventory/itemInventory") as GameObject, merchCont.transform.Find("stored"), false);
                money = merchCont.transform.Find("playerMoney").gameObject;

                money.GetComponent<Text>().text = gameControl.control.money.ToString();

                //show every item in inventory
                if (invItems != null)
                    foreach (Item item in invItems)
                        AddItem(item.name, item.sellValue, ownedItems, buttonScript.buttonType.sell);

                //show every item on sale
                if (merchItems != null)
                    foreach (Item merchitem in merchItems)
                        AddItem(merchitem.name, merchitem.value, shopItems, buttonScript.buttonType.buy);
            }
            else
            {
                merchCont.SetActive(true);
            }

            //checks if anything has changed in lists
            foreach (Transform child in shopItems.transform)
            {
                Item temp = merchItems.FirstOrDefault(i => i.name == child.name);
                if (temp == null)
                    Destroy(child.gameObject);
            }

            foreach (Item itemInInventory in invItems)
                if (!ownedItems.transform.Find(itemInInventory.name))
                    AddItem(itemInInventory.name, itemInInventory.sellValue, ownedItems, buttonScript.buttonType.sell);

            foreach (Item mItem in merchItems)
                if (!shopItems.transform.Find(mItem.name))
                    AddItem(mItem.name, mItem.value, shopItems, buttonScript.buttonType.buy);

            CheckDuplicates(ownedItems, invItems);
            CheckEquipAndValue();



            //check if item is removed and remove it from the list
            invItems.RemoveAll(Item => Item == null);
        }
        else
        {
            //close this inventory
            merch = false;
            anyOpen = false;
            merchCont.SetActive(false);
            merchClose = false;
        }


        //toggles timescale
        PauseNoMenu();
    }

    public void MerchClick(string name, bool buy)
    {
        //buying
        if (buy)
        {
            Item tempItem = merchItems.FirstOrDefault(i => i.name == name);
            if (tempItem != null)
            {
                merchItems.Remove(tempItem);
                invItems.Add(tempItem);

                gameControl.control.money -= tempItem.value;

                //deletes from list
                foreach (Transform child in shopItems.transform)
                {
                    Item temp = merchItems.FirstOrDefault(i => i.name == child.name);
                    if (temp == null)
                        Destroy(child.gameObject);
                }

                //adds to another list
                foreach (Item itemInInventory in invItems)
                    if (!ownedItems.transform.Find(itemInInventory.name))
                        AddItem(itemInInventory.name, itemInInventory.sellValue, ownedItems, buttonScript.buttonType.sell);
            }
        }
        //selling
        else
        {
            if (name == equipOne || name == equipTwo)
            {
                print("equipped, cannot sell");
            }
            else
            {
                Item tempItem = invItems.FirstOrDefault(i => i.name == name);
                if (tempItem != null)
                {
                    if (tempItem.canSell)
                    {
                        invItems.Remove(tempItem);
                        merchItems.Add(tempItem);

                        //deletes from list
                        foreach (Transform child in ownedItems.transform)
                        {
                            Item temp = invItems.FirstOrDefault(i => i.name == child.name);
                            if (temp == null)
                                Destroy(child.gameObject);
                        }

                        //adds to another list
                        foreach (Item mItem in merchItems)
                            if (!shopItems.transform.Find(mItem.name))
                                AddItem(mItem.name, mItem.value, shopItems, buttonScript.buttonType.buy);

                        gameControl.control.money += tempItem.sellValue;
                    }
                    else
                    {
                        print("can't sell");
                    }
                }
            }
        }
        CheckDuplicates(ownedItems, invItems);
        CheckEquipAndValue();
    }

    //checks if item is equipped or too expensive
    void CheckEquipAndValue()
    {
        money.GetComponent<Text>().text = gameControl.control.money.ToString();

        //changes color of already equipped item
        foreach (Item item in invItems)
        {
            if (item.name == equipOne || item.name == equipTwo || !item.canSell)
            {
                ownedItems.transform.Find(item.name).GetComponent<Image>().color = Color.gray;
            }
            else
            {
                ownedItems.transform.Find(item.name).GetComponent<Image>().color = Color.white;
            }
        }

        foreach (Item item in merchItems)
        {
            if (item.value > gameControl.control.money)
            {
                shopItems.transform.Find(item.name).GetComponent<Image>().color = Color.gray;
                shopItems.transform.Find(item.name).GetComponent<buttonScript>().type = buttonScript.buttonType.expensive;
            }
            else
            {
                shopItems.transform.Find(item.name).GetComponent<Image>().color = Color.white;
                shopItems.transform.Find(item.name).GetComponent<buttonScript>().type = buttonScript.buttonType.buy;
            }
        }
    }

    public void TooExpensive()
    {
        print("item is too expensive");
        //sound and/or animation here
    }

    //show image and name of collected item
    public static void ShowCollected(string collectedName)
    {
        showC = true;
        showCollectibleTime = 0;

        if (collected == null)
        {
            collected = Instantiate(Resources.Load("ui/collected") as GameObject, GameObject.Find("Canvas").transform, false);
            //collected.transform.Find("itemImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/inventory/sprites/" + collectedName);
            collected.transform.Find("collectedName").GetComponent<Text>().text = collectedName;
        }
        else
        {
            collected.SetActive(true);
            //collected.transform.Find("itemImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/inventory/sprites/" + collectedName);
            collected.transform.Find("collectedName").GetComponent<Text>().text = collectedName;
        }
    }

    public void HideCollected()
    {
        collected.SetActive(false);
    }

    public static Transform[] FindChildren(Transform tr, string name)
    {
        return tr.GetComponentsInChildren<Transform>().Where(t => t.name == name).ToArray();
    }
}

