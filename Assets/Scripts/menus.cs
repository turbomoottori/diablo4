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
    /*
    //FRIENDLY REMINDER THAT YOU ARE ACTUALLY WORKING WITH ANOTHER SCRIPT U DUMBASS
    //THEY'RE NAMED "newCollectible" AND "items" SO GO CHECK 'EM OUT
    //JUST LEAVE THIS ONE ALONE PLS AND FIX YOUR SHIT
    // >:(

    //actually now that we're at it why don't you make the whole fucking ui from scracth
    //since it sucks ass rn

    //also remember to delete this rant

    GameObject playerTalk, playerChoice;
    GameObject talkBox, talkText, ansCont;
    Image healthbar, batteryHp;
    public static bool chestClose, merchClose, talkReady, bcClose = false;
    public static bool anyOpen = false;
    public bool choice = false;

    bool talks = false;
    public static bool pauseOpen, invOpen, stInvOpen, merch, bcOpen, textShown, newbookstxt = false;
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
    GameObject inventoryContainer, itemContainer, chestContainer, itemsInInventory, itemsInChest;
    GameObject merchCont, ownedItems, shopItems, money, bookcasebg, bookTxt, newBooks;
    public Vector3[] bookPositions;
    List<Item> merchItems = new List<Item>();
    //public static List<Book> bookcaseBooks = new List<Book>();
    //public static string equipOne, equipTwo;

    void Start()
    {
        canv = GameObject.Find("Canvas").transform;
        healthbar = Instantiate(Resources.Load("ui/healthBar") as GameObject, canv).transform.GetChild(0).GetChild(0).GetComponent<Image>();

        volumeVal = gameControl.control.volume;

        talkBox = Instantiate(Resources.Load("ui/speechBox") as GameObject, canv);
        talkBox.SetActive(false);
        talkText = Instantiate(Resources.Load("ui/speechText") as GameObject, talkBox.transform.GetChild(0).transform);
        ansCont = Instantiate(Resources.Load("ui/answerCont") as GameObject, talkBox.transform.GetChild(0).transform);
    }

    private void Update()
    {
        //INTERACT
        if (Input.GetKeyDown(KeyCode.E) && !pauseOpen && !invOpen)
        {
            //TOGGLE CHEST
            if (chestClose)
                StoredItems();
            //TOGGLE MERCH ITEMS
            else if (merchClose)
                MerchantUI();
            //SCROLL TEXT
            else if (talkReady && !choice)
                Talk();
            //TOGGLE BOOKCASE
            else if (bcClose && !textShown && !newbookstxt)
                Bookcase();
            //HIDE BOOK TEXT
            else if (textShown)
                CloseBookText();
            //HIDE TEXT TELLING ABOUT NEW BOOKS
            else if (newbookstxt)
                HideAddedBooksText();
        }

        //PRESSING ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!anyOpen || pauseOpen)
                TogglePause();
            else if (invOpen)
                Inventory();
            else if (talks && !choice)
                Talk();
            else if (stInvOpen)
                StoredItems();
            else if (merch)
                MerchantUI();
            else if (bcOpen && !textShown && !newbookstxt)
                Bookcase();
            else if (textShown)
                CloseBookText();
            else if (newbookstxt)
                HideAddedBooksText();
        }

        //if player is talking or reading a book, click will close or scroll
        if (anyOpen && Input.GetButtonDown("Fire1"))
        {
            if (!pauseOpen && !invOpen && talks && !stInvOpen && !merch && !choice && !bcOpen && !textShown)
                Talk();
            else if (textShown)
                CloseBookText();
        }

        //OPEN INVENTORY
        if (Input.GetKeyDown(KeyCode.I) && (!anyOpen || invOpen))
            Inventory();

        // HEALTH
        temphp = gameControl.control.hp;
        tempmaxhp = gameControl.control.maxhp;
        healthbar.fillAmount = (float)temphp / (float)tempmaxhp;

        //COLLECTED ITEM   
        showCollectibleTime += Time.deltaTime;
        if (showCollectibleTime >= 4 && showC)
        {
            showC = false;
            collected.SetActive(false);
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

                //if there's consequenses without choice, invoke them
                if (tempSpeak[currentPage].answer.consequences.Length > 0 && !choice)
                    tempSpeak[currentPage].answer.consequences[0].Invoke();
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

    //when clicked on a button in pause menu
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

    //toggle inventory
    public void Inventory()
    {
        if (!invOpen)
        {
            invOpen = true;
            anyOpen = true;

            PauseNoMenu();

            //create inventory menu if it doesn't exist
            if (inventoryContainer == null)
            {
                inventoryContainer = Instantiate(Resources.Load("ui/inventory/inventory") as GameObject, canv, false);
                itemContainer = Instantiate(Resources.Load("ui/inventory/itemInventory") as GameObject, inventoryContainer.transform.Find("items").transform, false);

                //show every item in inventory
                if (gameControl.invItems != null)
                {
                    foreach (Item item in gameControl.invItems)
                    {
                        AddItem(item, itemContainer, buttonScript.buttonType.equip);
                    }
                }
                        

                if (gameControl.equipOne == null)
                    gameControl.equipOne = "Empty";
                if (gameControl.equipTwo == null)
                    gameControl.equipTwo = "Empty";

                //display equipped items
                AddEquipped(gameControl.equipOne, 1);
                AddEquipped(gameControl.equipTwo, 2);

                GameObject btr = Instantiate(Resources.Load("ui/inventory/battery") as GameObject, inventoryContainer.transform.Find("active").transform, false);
                batteryHp = btr.transform.Find("slot").transform.Find("BatteryHpContainer").transform.Find("batteryHp").GetComponent<Image>();
                btr.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(BatteryToggle);
                btr.transform.Find("Toggle").GetComponent<Toggle>().isOn = gameControl.control.autoBattery;

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
                inventoryContainer.SetActive(true);
                if(gameControl.invItems.FirstOrDefault(i=>i.name == gameControl.equipOne) is Gun)
                    EquippedGunType(1);
                if (gameControl.invItems.FirstOrDefault(i => i.name == gameControl.equipTwo) is Gun)
                    EquippedGunType(2);
            }

            //if there's new items add them too
            foreach (Item itemInInventory in gameControl.invItems)
                if (!itemContainer.transform.Find(itemInInventory.name))
                    AddItem(itemInInventory, itemContainer, buttonScript.buttonType.equip);

            //changes color of already equipped item
            foreach (Item item in gameControl.invItems)
            {
                if (item.name == gameControl.equipOne || item.name == gameControl.equipTwo || item.name == battery.batteryName)
                    itemContainer.transform.Find(item.name).GetComponent<Image>().color = Color.gray;
                else
                    itemContainer.transform.Find(item.name).GetComponent<Image>().color = Color.white;
            }

            if (quests.questList != null)
                CheckQuests();

            //checks duplicates
            CheckDuplicates(itemContainer, gameControl.invItems);

            UpdateBatteries(itemContainer);
            DisplayBatteryEnergy();

            gameControl.invItems.RemoveAll(Item => Item == null);
        }
        //closes inventory
        else if (invOpen)
        {
            invOpen = false;
            anyOpen = false;
            inventoryContainer.SetActive(false);
            PauseNoMenu();
        }
    }

    void DisplayBatteryEnergy()
    {
        //shows battery energy
        if (battery.batteryOn)
        {
            foreach (Item i in gameControl.invItems)
            {
                if(i is Battery)
                {
                    Battery bt = i as Battery;
                    if (bt.id == gameControl.batteryId)
                        batteryHp.fillAmount = bt.energy;
                }
            }
        }
        else
        {
            batteryHp.fillAmount = 0f;
        }
    }

    void UpdateBatteries(GameObject container)
    {
        foreach (Transform child in container.transform)
        {
            if (child.name.Contains("Battery"))
            {
                if (container == itemContainer || container == ownedItems)
                {
                    Battery b = gameControl.invItems.FirstOrDefault(i => i.name == child.name) as Battery;
                    print(b.name);
                    child.transform.Find("name").GetComponent<Text>().text = "Battery " + (b.energy / 1 * 100).ToString("F0") + "%";
                }
                else if (container == shopItems)
                {
                    Battery b = merchItems.FirstOrDefault(i => i.name == child.name) as Battery;
                    child.transform.Find("name").GetComponent<Text>().text = "Battery " + (b.energy / 1 * 100).ToString("F0") + "%";
                }
                else if (container == chestContainer)
                {
                    Battery b = gameControl.itemsStored.FirstOrDefault(i => i.name == child.name) as Battery;
                    child.transform.Find("name").GetComponent<Text>().text = "Battery " + (b.energy / 1 * 100).ToString("F0") + "%";
                }
            }
        }
    }

    void AddItem(Item item, GameObject place, buttonScript.buttonType type)
    {
        GameObject i = Instantiate(Resources.Load("ui/inventory/item") as GameObject, place.transform, false);
        if(item is Battery)
        {
            Battery b = item as Battery;
            i.transform.Find("name").GetComponent<Text>().text = "Battery " + (b.energy/1*100).ToString("F0")+ "%";
            i.name = item.name;
        }
        else
        {
            i.transform.Find("name").GetComponent<Text>().text = item.name;
            i.name = item.name;
        }

        if (place == ownedItems)
            i.transform.Find("weight").GetComponent<Text>().text = item.sellValue.ToString();
        else if (place == shopItems)
            i.transform.Find("weight").GetComponent<Text>().text = item.value.ToString();
        else
            i.transform.Find("weight").GetComponent<Text>().text = item.weight.ToString();

        i.GetComponent<buttonScript>().type = type;
    }

    //display equipped items
    void AddEquipped(string name, int equipNumber)
    {
        GameObject equip = Instantiate(Resources.Load("ui/inventory/equipped") as GameObject, inventoryContainer.transform.Find("active").transform, false);
        equip.transform.Find("title").GetComponent<Text>().text = name;
        equip.name = name + equipNumber.ToString();

        //checks if equip is a gun, and if yes, show ammo
        Weapon w = gameControl.invItems.FirstOrDefault(i => i.name == name) as Weapon;
        if (w is Gun)
            equip.transform.Find("ammo").gameObject.SetActive(true);
        else
            equip.transform.Find("ammo").gameObject.SetActive(false);

        //equip.transform.Find("slot").GetComponent<Image>().sprite = Resources.Load<Sprite>("ui/inventory/sprites/" + name);
    }

    //adds quests to list
    void AddQuest(string name, string desc)
    {
        GameObject q = Instantiate(Resources.Load("ui/inventory/questblock") as GameObject, inventoryContainer.transform.Find("quests").transform, false);
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
            if (!inventoryContainer.transform.Find("quests").transform.Find("quest" + activeQuest.questName))
                AddQuest(activeQuest.questName, activeQuest.questDesc);


        string completedText = "DONE";
        foreach (Quest quest in quests.questList)
        {
            if (quest.completed)
            {
                inventoryContainer.transform.Find("quests").transform.Find("quest" + quest.questName).transform.Find(quest.questName + "desc").GetComponent<Text>().text = completedText;
            }
        }
    }

    //equipping weapons
    public void InventoryClick(bool leftClick, string name)
    {
        //assigning weapon 1
        if (leftClick)
        {
            foreach (Item item in gameControl.invItems)
            {
                //equips only if item is a weapons
                if (item.name == name && item is Weapon)
                {
                    if (name == gameControl.equipOne || name == gameControl.equipTwo)
                    {
                        //this item is already equipped
                        print("already equipped");
                    }
                    else
                    {
                        if (gameControl.equipOne == "Empty")
                            inventoryContainer.transform.Find("active").transform.Find(gameControl.equipOne + "1").name = name;
                        else
                            inventoryContainer.transform.Find("active").transform.Find(gameControl.equipOne).name = name;

                        inventoryContainer.transform.Find("active").transform.Find(name).transform.Find("title").GetComponent<Text>().text = name;
                        gameControl.equipOne = name;

                        if (item is Gun)
                        {
                            weapons.weaponType1 = 2;
                            //recast as gun
                            Gun g = item as Gun;
                            weapons.damage1 = g.damage;
                            weapons.speed1 = g.speed;
                            weapons.range1 = g.range;
                            weapons.type1 = g.type;
                            weapons.special1 = g.special;
                            weapons.rlspeed1 = g.rlspeed;
                            EquippedGunType(1);
                        }
                        else
                        {
                            //item is a sword
                            weapons.weaponType1 = 1;
                            //recast as weapon
                            Weapon w = item as Weapon;
                            weapons.damage1 = w.damage;
                            weapons.speed1 = w.speed;
                            inventoryContainer.transform.Find("active").transform.Find(gameControl.equipOne).transform.Find("ammo").gameObject.SetActive(false);
                        }
                    }
                }
                else if(item.name == name && item is Battery)
                {
                    Battery b = item as Battery;
                    if (!b.isEmpty)
                    {
                        GameObject.Find("Globals").GetComponent<battery>().UseSpecificBattery(b);
                        battery.batteryName = item.name;
                    }
                    else
                    {
                        print("this battery is empty");
                    }
                }
            }
        }
        //assigning weapon 2
        else
        {
            foreach (Item item in gameControl.invItems)
            {
                if (item.name == name && item is Weapon)
                {
                    if (name == gameControl.equipTwo || name == gameControl.equipOne)
                    {
                        print("already equipped");
                    }
                    else
                    {
                        if (gameControl.equipTwo == "Empty")
                            inventoryContainer.transform.Find("active").transform.Find(gameControl.equipTwo + "2").name = name;
                        else
                            inventoryContainer.transform.Find("active").transform.Find(gameControl.equipTwo).name = name;

                        inventoryContainer.transform.Find("active").transform.Find(name).transform.Find("title").GetComponent<Text>().text = name;
                        gameControl.equipTwo = name;

                        if (item is Gun)
                        {
                            weapons.weaponType2 = 2;
                            //recast as gun
                            Gun g = item as Gun;
                            weapons.damage2 = g.damage;
                            weapons.speed2 = g.speed;
                            weapons.range2 = g.range;
                            weapons.type2 = g.type;
                            weapons.special2 = g.special;
                            weapons.rlspeed2 = g.rlspeed;
                            EquippedGunType(2);
                        }
                        else
                        {
                            weapons.weaponType2 = 1;
                            //recast as weapon
                            Weapon w = item as Weapon;
                            weapons.damage2 = w.damage;
                            weapons.speed2 = w.speed;
                            inventoryContainer.transform.Find("active").transform.Find(gameControl.equipTwo).transform.Find("ammo").gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        foreach (Transform child in itemContainer.transform)
        {
            if (child.name == gameControl.equipOne || child.name == gameControl.equipTwo || child.name==battery.batteryName)
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
            Item tempItem = gameControl.itemsStored.FirstOrDefault(i => i.name == name);
            if (tempItem != null)
            {
                gameControl.itemsStored.Remove(tempItem);
                gameControl.invItems.Add(tempItem);

                //adds to another list
                foreach (Item itemInInventory in gameControl.invItems)
                    if (!itemsInInventory.transform.Find(itemInInventory.name))
                        AddItem(itemInInventory, itemsInInventory, buttonScript.buttonType.storable);
            }
        }
        //taking from inventory to storage
        else
        {
            if (name == gameControl.equipOne || name == gameControl.equipTwo || name == battery.batteryName)
            {
                print("equipped, cannot store");
            }
            else
            {
                Item tempItem = gameControl.invItems.FirstOrDefault(i => i.name == name);
                if (tempItem != null)
                {
                    gameControl.invItems.Remove(tempItem);
                    gameControl.itemsStored.Add(tempItem);

                    //adds to another list
                    foreach (Item itemStored in gameControl.itemsStored)
                        if (!itemsInChest.transform.Find(itemStored.name))
                            AddItem(itemStored, itemsInChest, buttonScript.buttonType.stored);
                }
            }
        }

        CheckDuplicates(itemsInInventory, gameControl.invItems);
        CheckDuplicates(itemsInChest, gameControl.itemsStored);
    }

    //opens and closes storage ui 
    public void StoredItems()
    {
        //opens storage ui
        if (!stInvOpen)
        {
            stInvOpen = true;
            anyOpen = true;

            if (chestContainer == null)
            {
                chestContainer = Instantiate(Resources.Load("ui/inventory/storeInv") as GameObject, canv, false);
                itemsInInventory = Instantiate(Resources.Load("ui/inventory/itemInventory") as GameObject, chestContainer.transform.Find("items"), false);
                itemsInChest = Instantiate(Resources.Load("ui/inventory/itemInventory") as GameObject, chestContainer.transform.Find("stored"), false);

                //show every item in inventory
                if (gameControl.invItems != null)
                    foreach (Item item in gameControl.invItems)
                        AddItem(item, itemsInInventory, buttonScript.buttonType.storable);

                //show every stored item
                if (gameControl.itemsStored != null)
                    foreach (Item stitem in gameControl.itemsStored)
                        AddItem(stitem, itemsInChest, buttonScript.buttonType.stored);
            }
            else
            {
                chestContainer.SetActive(true);
            }

            foreach (Item itemInInventory in gameControl.invItems)
                if (!itemsInInventory.transform.Find(itemInInventory.name))
                    AddItem(itemInInventory, itemsInInventory, buttonScript.buttonType.storable);

            foreach (Item itemStored in gameControl.itemsStored)
                if (!itemsInChest.transform.Find(itemStored.name))
                    AddItem(itemStored, itemsInChest, buttonScript.buttonType.stored);

            //changes color of already equipped item
            foreach (Item item in gameControl.invItems)
            {
                if (item.name == gameControl.equipOne || item.name == gameControl.equipTwo || item.name == battery.batteryName)
                {
                    itemsInInventory.transform.Find(item.name).GetComponent<Image>().color = Color.gray;
                }
                else
                {
                    itemsInInventory.transform.Find(item.name).GetComponent<Image>().color = Color.white;
                }
            }

            UpdateBatteries(itemsInChest);
            CheckDuplicates(itemsInChest, gameControl.itemsStored);
            CheckDuplicates(itemsInInventory, gameControl.invItems);


            //check if item is removed and remove it from the list
            gameControl.invItems.RemoveAll(Item => Item == null);
        }
        else
        {
            //close this inventory
            stInvOpen = false;
            anyOpen = false;
            chestContainer.SetActive(false);
            chestClose = false;
        }

        //toggles timescale
        PauseNoMenu();
    }

    //show bookcase UI
    public void Bookcase()
    {
        //open bookcase
        if(!bcOpen)
        {
            bcOpen = true;
            anyOpen = true;

            if (bookcasebg == null)
            {
                bookcasebg = Instantiate(Resources.Load("ui/bookcase/bookcasebg") as GameObject, canv, false);

                //shows already existing books
                if (gameControl.bookcaseBooks != null)
                    foreach (Book b in gameControl.bookcaseBooks)
                        AddBooks(b.id, b.name);
            }
            else
            {
                bookcasebg.SetActive(true);
            }
                

            int bookAmount = 0;
            //adds every book in inventory to the bookcase
            foreach (Item i in gameControl.invItems)
            {
                if (i is Book)
                {
                    bookAmount += 1;
                    Book b = i as Book;
                    AddBooks(b.id, b.name);
                    gameControl.bookcaseBooks.Add(b);
                }
            }

            //tells the amount of added books
            if (bookAmount > 0)
                BooksAdded(bookAmount);

            //removes all listed books from inventory
            gameControl.invItems.RemoveAll(i => gameControl.bookcaseBooks.Exists(b => i.name == b.name));

            PauseNoMenu();
        }
        //close bookcase
        else
        {
            bcOpen = false;
            anyOpen = false;
            bcClose = false;
            bookcasebg.SetActive(false);
            PauseNoMenu();
        }
    }

    //adds book to bookcase
    void AddBooks(int id, string n)
    {
        GameObject book;
        book = Instantiate(Resources.Load("ui/bookcase/book") as GameObject, bookcasebg.transform.Find("bookContainer").transform, false);
        book.GetComponent<RectTransform>().localPosition = bookPositions[id];
        book.name = n;
        book.GetComponent<buttonScript>().type = buttonScript.buttonType.book;
    }

    //shows text from book
    public void ClickBook(string name)
    {
        if (bookTxt == null)
            bookTxt = Instantiate(Resources.Load("ui/bookcase/bookText") as GameObject, bookcasebg.transform, false);
        else
            bookTxt.SetActive(true);

        Book book = gameControl.bookcaseBooks.FirstOrDefault(b => b.name == name);
        bookTxt.transform.GetChild(0).GetComponent<Text>().text = book.txt;
        textShown = true;
    }
    
    //closes book text
    public void CloseBookText()
    {
        bookTxt.SetActive(false);
        textShown = false;
    }

    //changes merchant items
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
                if (gameControl.invItems != null)
                    foreach (Item item in gameControl.invItems)
                        AddItem(item, ownedItems, buttonScript.buttonType.sell);

                //show every item on sale
                if (merchItems != null)
                    foreach (Item merchitem in merchItems)
                        AddItem(merchitem, shopItems, buttonScript.buttonType.buy);
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

            foreach (Item itemInInventory in gameControl.invItems)
                if (!ownedItems.transform.Find(itemInInventory.name))
                    AddItem(itemInInventory, ownedItems, buttonScript.buttonType.sell);

            foreach (Item mItem in merchItems)
                if (!shopItems.transform.Find(mItem.name))
                    AddItem(mItem, shopItems, buttonScript.buttonType.buy);

            UpdateBatteries(ownedItems);
            UpdateBatteries(shopItems);
            CheckDuplicates(ownedItems, gameControl.invItems);
            CheckEquipAndValue();



            //check if item is removed and remove it from the list
            gameControl.invItems.RemoveAll(Item => Item == null);
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
                gameControl.invItems.Add(tempItem);

                gameControl.control.money -= tempItem.value;

                //deletes from list
                foreach (Transform child in shopItems.transform)
                {
                    Item temp = merchItems.FirstOrDefault(i => i.name == child.name);
                    if (temp == null)
                        Destroy(child.gameObject);
                }

                //adds to another list
                foreach (Item itemInInventory in gameControl.invItems)
                    if (!ownedItems.transform.Find(itemInInventory.name))
                        AddItem(itemInInventory, ownedItems, buttonScript.buttonType.sell);
            }
        }
        //selling
        else
        {
            if (name == gameControl.equipOne || name == gameControl.equipTwo || name == battery.batteryName)
            {
                print("equipped, cannot sell");
            }
            else
            {
                Item tempItem = gameControl.invItems.FirstOrDefault(i => i.name == name);
                if (tempItem != null)
                {
                    if (tempItem.canSell)
                    {
                        gameControl.invItems.Remove(tempItem);
                        merchItems.Add(tempItem);

                        //deletes from list
                        foreach (Transform child in ownedItems.transform)
                        {
                            Item temp = gameControl.invItems.FirstOrDefault(i => i.name == child.name);
                            if (temp == null)
                                Destroy(child.gameObject);
                        }

                        //adds to another list
                        foreach (Item mItem in merchItems)
                            if (!shopItems.transform.Find(mItem.name))
                                AddItem(mItem, shopItems, buttonScript.buttonType.buy);

                        gameControl.control.money += tempItem.sellValue;
                    }
                    else
                    {
                        print("can't sell");
                    }
                }
            }
        }
        CheckDuplicates(ownedItems, gameControl.invItems);
        CheckEquipAndValue();
    }

    //checks if item is equipped or too expensive
    void CheckEquipAndValue()
    {
        money.GetComponent<Text>().text = gameControl.control.money.ToString();

        //changes color of already equipped item
        foreach (Item item in gameControl.invItems)
        {
            if (item.name == gameControl.equipOne || item.name == gameControl.equipTwo || !item.canSell || item.name == battery.batteryName)
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

    //show text that tells how many books have been added
    void BooksAdded(int howMany)
    {
        newbookstxt = true;

        if (newBooks == null)
            newBooks = Instantiate(Resources.Load("ui/bookcase/addedbooks") as GameObject, bookcasebg.transform, false);
        else
            newBooks.SetActive(true);

        if (howMany == 1)
            newBooks.transform.Find("txt").GetComponent<Text>().text = "1 book added";
        else
            newBooks.transform.Find("txt").GetComponent<Text>().text = howMany.ToString() + " books added";
    }

    //hide text telling about added books
    void HideAddedBooksText()
    {
        newbookstxt = false;
        newBooks.SetActive(false);
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

    void EquippedGunType(int equip)
    {
        Gun tempGun;
        string tempAmmo;

        if(equip == 1)
        {
            tempGun = gameControl.invItems.FirstOrDefault(i => i.name == gameControl.equipOne) as Gun;
            inventoryContainer.transform.Find("active").transform.Find(gameControl.equipOne).transform.Find("ammo").gameObject.SetActive(true);
            if (tempGun.type == "normal")
                tempAmmo = gameControl.basicAmmo.ToString();
            else if (tempGun.type == "shotgun")
                tempAmmo = gameControl.shotgunAmmo.ToString();
            else
                tempAmmo = gameControl.rapidAmmo.ToString();

            inventoryContainer.transform.Find("active").transform.Find(gameControl.equipOne).transform.Find("ammo").GetComponent<Text>().text = tempAmmo;
        }
        else
        {
            tempGun = gameControl.invItems.FirstOrDefault(i => i.name == gameControl.equipTwo) as Gun;
            inventoryContainer.transform.Find("active").transform.Find(gameControl.equipTwo).transform.Find("ammo").gameObject.SetActive(true);
            if (tempGun.type == "normal")
                tempAmmo = gameControl.basicAmmo.ToString();
            else if (tempGun.type == "shotgun")
                tempAmmo = gameControl.shotgunAmmo.ToString();
            else
                tempAmmo = gameControl.rapidAmmo.ToString();

            inventoryContainer.transform.Find("active").transform.Find(gameControl.equipOne).transform.Find("ammo").GetComponent<Text>().text = tempAmmo;
        }
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

    void BatteryToggle(bool toggleOn)
    {
        if (toggleOn)
        {
            gameControl.control.autoBattery = true;
        }
        else
        {
            gameControl.control.autoBattery = false;
        }
    }

    public static Transform[] FindChildren(Transform tr, string name)
    {
        return tr.GetComponentsInChildren<Transform>().Where(t => t.name == name).ToArray();
    }*/
}

