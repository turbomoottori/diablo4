﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ui : MonoBehaviour {

    Transform canv;
    GameObject inv, hp, pausemenu, savecaution, chest, merchant, dBox, bookcase, newItem;
    GameObject itemContainer, chest_itemContainer, chest_storedContainer, merchant_owned, merchant_selling, dNpc, dPl;
    Text merchant_playerMoney;
    GameObject[] pauseWindows, answers, books;
    KeyCode keyInventory, keyInteract;
    public static bool anyOpen;
    bool choice, bookReading;
    public static GameObject interactableObject;
    public static List<Item> merchantItems;
    public static Dialogue[] npcDialogue;
    int currentPage;
    float showItemTimer = 0f;
    int tempHp;

	void Start () {
        canv = GameObject.Find("Canvas").transform;
        DefaultKeys();
        LoadUI();
        tempHp = gameControl.control.hp;
	}

    //sets default keys
    void DefaultKeys()
    {
        keyInventory = KeyCode.Tab;
        keyInteract = KeyCode.E;
    }
	
	void Update () {

        if (showItemTimer < 2f)
            showItemTimer += Time.deltaTime;
        if (showItemTimer > 2f)
            newItem.SetActive(false);

        if (gameControl.control.hp != tempHp)
        {
            hp.transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = (float)gameControl.control.hp / (float)gameControl.control.maxhp;
            tempHp = gameControl.control.hp;
        }

        switch (anyOpen)
        {
            case true:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (inv.activeInHierarchy)
                        Close(inv);
                    if (pauseWindows[1].activeInHierarchy || pauseWindows[2].activeInHierarchy || pauseWindows[3].activeInHierarchy)
                        CloseAllPauseMenusExcept(0);
                    if (pauseWindows[0].activeInHierarchy)
                        Close(pausemenu);
                    if (chest.activeInHierarchy)
                        Close(chest);
                    if (merchant.activeInHierarchy)
                        Close(merchant);
                    if (dBox.activeInHierarchy && !choice)
                        Dialogue();
                    if (bookcase.activeInHierarchy && !bookReading && !bookcase.transform.Find("addedbooks").gameObject.activeInHierarchy)
                        Close(bookcase);
                    if (bookcase.transform.Find("addedbooks").gameObject.activeInHierarchy)
                        bookcase.transform.Find("addedbooks").gameObject.SetActive(false);
                }

                if (Input.GetKeyDown(keyInventory) || Input.GetKeyDown(keyInteract))
                {
                    if(inv.activeInHierarchy)
                        Close(inv);
                    if (merchant.activeInHierarchy)
                        Close(merchant);
                    if (chest.activeInHierarchy)
                        Close(chest);
                    if (bookcase.activeInHierarchy && !bookReading && !bookcase.transform.Find("addedbooks").gameObject.activeInHierarchy)
                        Close(bookcase);
                    if (bookcase.transform.Find("addedbooks").gameObject.activeInHierarchy)
                        bookcase.transform.Find("addedbooks").gameObject.SetActive(false);
                    if (bookReading)
                    {
                        bookcase.transform.Find("bookText").gameObject.SetActive(false);
                        bookReading = false;
                    }
                }

                if ((dBox.activeInHierarchy && !choice) && (Input.GetKeyDown(keyInteract) || Input.GetMouseButtonDown(0)))
                    Dialogue();

                if (bookcase.activeInHierarchy && Input.GetMouseButtonDown(0))
                {
                    if (bookcase.transform.Find("addedbooks").gameObject.activeInHierarchy)
                        bookcase.transform.Find("addedbooks").gameObject.SetActive(false);
                    if (bookReading)
                    {
                        bookcase.transform.Find("bookText").gameObject.SetActive(false);
                        bookReading = false;
                    }
                }

                break;
            case false:

                if (Input.GetKeyDown(keyInventory))
                    OpenInventory();
                if (Input.GetKeyDown(KeyCode.Escape))
                    OpenPauseMenu();
                if (Input.GetKeyDown(keyInteract))
                {
                    if (interactableObject != null)
                    {
                        switch (interactableObject.gameObject.GetComponent<interactable>().type)
                        {
                            case interactable.Type.merchant:
                                OpenMerchantWindow();
                                break;
                            case interactable.Type.collectible:
                                interactableObject.GetComponent<newCollectible>().PickUp();
                                break;
                            case interactable.Type.chest:
                                OpenChestWindow();
                                break;
                            case interactable.Type.npc:
                                npcDialogue = interactableObject.GetComponent<npc>().dialogue;
                                currentPage = 0;
                                Dialogue();
                                interactableObject.GetComponent<npc>().Interact();
                                break;
                            case interactable.Type.bookcase:
                                OpenBookcase();
                                break;
                        }
                    }
                }
                break;
        }
	}

    void OpenInventory()
    {
        anyOpen = true;
        inv.SetActive(true);
        TogglePause();
        ShowItems(itemContainer, items.ownedItems, buttonScript.buttonType.inventoryItem);
        ShowEquips();
    }

    void OpenPauseMenu()
    {
        anyOpen = true;
        pausemenu.SetActive(true);
        TogglePause();
        CloseAllPauseMenusExcept(0);
    }

    void OpenChestWindow()
    {
        anyOpen = true;
        chest.SetActive(true);
        TogglePause();
        ShowItems(chest_itemContainer, items.ownedItems, buttonScript.buttonType.readyToStore);
        ShowItems(chest_storedContainer, items.storedItems, buttonScript.buttonType.readyToTake);
    }

    void OpenMerchantWindow()
    {
        anyOpen = true;
        merchant.SetActive(true);
        TogglePause();
        ShowItems(merchant_owned, items.ownedItems, buttonScript.buttonType.readyToSell);
        ShowItems(merchant_selling, merchantItems, buttonScript.buttonType.readyToBuy);
        CheckValuesAndEquips();
    }

    void OpenBookcase()
    {
        anyOpen = true;
        bookcase.SetActive(true);
        TogglePause();
        int newBooks = 0;

        foreach(Item i in items.ownedItems)
        {
            if(i is Book)
            {
                Book b = i as Book;
                newBooks += 1;
                items.books.Add(b);
                books[b.id].SetActive(true);
            }
        }

        items.ownedItems.RemoveAll(i => items.books.Exists(b => i.name == b.name));

        if (newBooks > 0)
        {
            bookcase.transform.Find("addedbooks").gameObject.SetActive(true);
            if (newBooks == 1)
                bookcase.transform.Find("addedbooks").GetChild(0).GetComponent<Text>().text = "1 new book added";
            else
                bookcase.transform.Find("addedbooks").GetChild(0).GetComponent<Text>().text = newBooks.ToString() + " new books added";
        }
    }

    void Dialogue()
    {
        int maxPages = npcDialogue.Length;

        if (currentPage < maxPages)
        {
            if (currentPage == 0)
            {
                anyOpen = true;
                dBox.SetActive(true);
                TogglePause();
            }

            //npc talks
            if (npcDialogue[currentPage].who == who.npc)
            {
                dPl.SetActive(false);
                dNpc.SetActive(true);
                dNpc.transform.Find("txt").GetComponent<Text>().text = npcDialogue[currentPage].npc.talk;

                if (npcDialogue[currentPage].npc.consequenseWithoutAction != null)
                    npcDialogue[currentPage].npc.consequenseWithoutAction.Invoke();

                currentPage += 1;
            }
            //player answers
            else
            {
                dPl.SetActive(true);
                dNpc.SetActive(false);
                choice = true;

                for(int i = 0; i < answers.Length; i++)
                {
                    if (i < npcDialogue[currentPage].player.Length)
                    {
                        answers[i].SetActive(true);
                        answers[i].transform.GetChild(0).GetComponent<Text>().text = npcDialogue[currentPage].player[i].answer;
                    }
                    else if (i >= npcDialogue[currentPage].player.Length)
                        answers[i].SetActive(false);
                }
            }
        }
        else
        {
            currentPage = 0;
            Close(dBox);
        }
    }

    //adds items to lists
    void AddItem(Item itemToAdd, GameObject container, buttonScript.buttonType type)
    {
        GameObject i = Instantiate(Resources.Load("ui/inventory/item2") as GameObject, container.transform, false);
        i.name = itemToAdd.name + itemToAdd.id.ToString();
        i.transform.Find("name").GetComponent<Text>().text = itemToAdd.name;
        i.GetComponent<buttonScript>().type = type;
    }

    //shows every item in list and checks duplicates
    void ShowItems(GameObject g, List<Item> l, buttonScript.buttonType type)
    {
        if (l != null)
        {
            foreach (Item i in l)
                if (!g.transform.Find(i.name))
                    AddItem(i, g, type);
        }

        foreach(Transform child in g.transform)
        {
            Item t = l.FirstOrDefault(i => i.name + i.id.ToString() == child.name);
            Transform[] duplicateCheck = FindChildren(g.transform, child.name);

            if (duplicateCheck.Length > 1)
            {
                for (int i = 0; i < duplicateCheck.Length; i++)
                    if (i != 0)
                        Destroy(duplicateCheck[i].gameObject);
            }

            //display amount if item is stackable
            if (t != null && t.stackable)
            {
                List<Item> tempList = l.FindAll(i => i.name + i.id.ToString() == child.name);
                string stacks = " (" + tempList.Count.ToString() + ")";
                child.transform.Find("name").GetComponent<Text>().text = t.name + stacks;
            }
            if (t == null)
                Destroy(child.gameObject);
        }
    }

    //checks merchant prices and equips
    void CheckValuesAndEquips()
    {
        foreach (Item i in items.ownedItems)
        {
            if (i == items.equippedOne || i == items.equippedTwo)
            {
                merchant_owned.transform.Find(i.name + i.id).GetComponent<Image>().color = Color.gray;
                merchant_owned.transform.Find(i.name + i.id).GetComponent<buttonScript>().type = buttonScript.buttonType.cantSell;
            }
            else
            {
                merchant_owned.transform.Find(i.name + i.id).GetComponent<Image>().color = Color.white;
                merchant_owned.transform.Find(i.name + i.id).GetComponent<buttonScript>().type = buttonScript.buttonType.readyToSell;
            }
        }

        foreach (Item i in merchantItems)
        {
            if (i.baseValue > gameControl.control.money)
            {
                merchant_selling.transform.Find(i.name + i.id).GetComponent<Image>().color = Color.gray;
                merchant_selling.transform.Find(i.name + i.id).GetComponent<buttonScript>().type = buttonScript.buttonType.cantBuy;
            }
            else
            {
                merchant_selling.transform.Find(i.name + i.id).GetComponent<Image>().color = Color.white;
                merchant_selling.transform.Find(i.name + i.id).GetComponent<buttonScript>().type = buttonScript.buttonType.readyToBuy;
            }
        }
    }

    //shows equipped items
    void ShowEquips()
    {
        GameObject e1, e2, b;
        e1 = inv.transform.Find("active").Find("equip1").gameObject;
        e2 = inv.transform.Find("active").Find("equip2").gameObject;
        b = inv.transform.Find("active").Find("battery").gameObject;

        if (items.equippedOne != null)
        {
            e1.transform.Find("title").GetComponent<Text>().text = items.equippedOne.name;
            if (items.equippedOne is Gun)
                e1.transform.Find("ammo").gameObject.SetActive(true);
            else
                e1.transform.Find("ammo").gameObject.SetActive(false);
        }
        else
        {
            e1.transform.Find("title").GetComponent<Text>().text = "Empty";
            e1.transform.Find("ammo").gameObject.SetActive(false);
        }

        if (items.equippedTwo != null)
        {
            e2.transform.Find("title").GetComponent<Text>().text = items.equippedTwo.name;
            if (items.equippedTwo is Gun)
                e2.transform.Find("ammo").gameObject.SetActive(true);
            else
                e2.transform.Find("ammo").gameObject.SetActive(false);
        }
        else
        {
            e2.transform.Find("title").GetComponent<Text>().text = "Empty";
            e2.transform.Find("ammo").gameObject.SetActive(false);
        }

        if (items.inUse != null)
        {
            b.transform.Find("slot").Find("BatteryHpContainer").GetChild(0).GetComponent<Image>().fillAmount = items.inUse.energy;
        }
        else
        {
            b.transform.Find("slot").Find("BatteryHpContainer").GetChild(0).GetComponent<Image>().fillAmount = 0;
        }
    }

    //closes a window
    void Close(GameObject windowToClose)
    {
        windowToClose.SetActive(false);
        anyOpen = false;
        TogglePause();
    }

    //toggles pause
    void TogglePause()
    {
        switch (playerMovement.paused)
        {
            case true:
                //unpauses
                playerMovement.paused = false;

                if (playerMovement.slowTime)
                {
                    Time.timeScale = 0.5f;
                    Time.fixedDeltaTime = 0.02f * Time.deltaTime;
                }
                else
                {
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = 0.02f;
                }

                break;
            case false:
                //pauses
                playerMovement.paused = true;
                Time.timeScale = 0;
                break;
        }
    }

    public void ClickedItem(bool leftClick, buttonScript.buttonType type, string name)
    {
        switch (type)
        {
            case buttonScript.buttonType.inventoryItem:
                Item t = items.ownedItems.FirstOrDefault(i => i.name + i.id == name);
                if (t != null)
                {
                    //equipping a weapon
                    if(t is Weapon)
                    {
                        Weapon w = t as Weapon;

                        if (leftClick)
                        {
                            //if some item is already equipped on slot 1
                            if (items.equippedOne != null)
                            {
                                if (items.equippedOne.name + items.equippedOne.id != name)
                                {
                                    if (items.equippedTwo != null)
                                    {
                                        if (items.equippedTwo.name + items.equippedTwo.id == name)
                                            items.equippedTwo = items.equippedOne;
                                    }

                                    items.equippedOne = w;
                                }
                            }
                            //slot 1 is empty
                            else
                            {
                                if (items.equippedTwo != null)
                                {
                                    if (items.equippedTwo.name + items.equippedTwo.id == name)
                                        items.equippedTwo = null;
                                }

                                items.equippedOne = w;
                            }
                        }
                        else if (!leftClick)
                        {
                            //if some item is already equipped in slot 2
                            if (items.equippedTwo != null)
                            {
                                if (items.equippedTwo.name + items.equippedTwo.id != name)
                                {
                                    if (items.equippedOne != null)
                                    {
                                        if (items.equippedOne.name + items.equippedOne.id == name)
                                            items.equippedOne = items.equippedTwo;
                                    }

                                    items.equippedTwo = w;
                                }
                            }
                            //slot 2 is empty
                            else
                            {
                                if (items.equippedOne != null)
                                {
                                    if (items.equippedOne.name + items.equippedOne.id == name)
                                        items.equippedOne = null;
                                }

                                items.equippedTwo = w;
                            }
                        }
                    }
                    else if(t is Battery)
                    {
                        print("is battery");
                    }
                }

                ShowEquips();
                break;

            case buttonScript.buttonType.readyToBuy:
                Item tempItem = merchantItems.FirstOrDefault(i => i.name + i.id == name);
                if (tempItem != null)
                {
                    merchantItems.Remove(tempItem);
                    items.ownedItems.Add(tempItem);
                    gameControl.control.money -= tempItem.baseValue;

                    foreach(Transform child in merchant_selling.transform)
                        if (child.name == tempItem.name + tempItem.id)
                            Destroy(child.gameObject);

                    foreach (Item inventoryItem in items.ownedItems)
                        if (!merchant_owned.transform.Find(inventoryItem.name + inventoryItem.id))
                            AddItem(inventoryItem, merchant_owned, buttonScript.buttonType.readyToSell);
                }
                CheckValuesAndEquips();
                break;

            case buttonScript.buttonType.readyToSell:
                tempItem = items.ownedItems.FirstOrDefault(i => i.name + i.id == name);
                if (tempItem != null)
                {
                    items.ownedItems.Remove(tempItem);
                    merchantItems.Add(tempItem);
                    gameControl.control.money += tempItem.baseValue;
                    ShowItems(merchant_owned, items.ownedItems, buttonScript.buttonType.readyToSell);
                    ShowItems(merchant_selling, merchantItems, buttonScript.buttonType.readyToBuy);
                }
                CheckValuesAndEquips();
                break;

            case buttonScript.buttonType.cantBuy:
                print("can't buy");
                break;

            case buttonScript.buttonType.cantSell:
                print("can't sell");
                break;

            case buttonScript.buttonType.readyToStore:
                tempItem = items.ownedItems.FirstOrDefault(i => i.name + i.id == name);
                if (tempItem != null)
                {
                    items.ownedItems.Remove(tempItem);
                    items.storedItems.Add(tempItem);
                    ShowItems(chest_itemContainer, items.ownedItems, buttonScript.buttonType.readyToStore);
                    ShowItems(chest_storedContainer, items.storedItems, buttonScript.buttonType.readyToTake);
                }
                break;

            case buttonScript.buttonType.readyToTake:
                tempItem = items.storedItems.FirstOrDefault(i => i.name + i.id == name);
                if (tempItem != null)
                {
                    items.storedItems.Remove(tempItem);
                    items.ownedItems.Add(tempItem);
                    ShowItems(chest_itemContainer, items.ownedItems, buttonScript.buttonType.readyToStore);
                    ShowItems(chest_storedContainer, items.storedItems, buttonScript.buttonType.readyToTake);
                }
                break;
        }
    }

    void PauseClickListener()
    {
        string btn = EventSystem.current.currentSelectedGameObject.name;
        string lastButton = "";

        if (!btn.Contains("saveslot"))
            savecaution.SetActive(false);

        if (btn == "continue")
            Close(pausemenu);
        else if (btn == "options")
            CloseAllPauseMenusExcept(1);
        else if (btn == "save")
            CloseAllPauseMenusExcept(2);
        else if (btn == "savequit")
            CloseAllPauseMenusExcept(3);
        else if (btn == "backButton")
            CloseAllPauseMenusExcept(0);
        else if (btn.Contains("saveslot"))
        {
            if (btn == "saveslot1")
            {
                if (File.Exists(Application.persistentDataPath + "/save1.dat") && lastButton!=btn)
                {
                    savecaution.SetActive(true);
                }
                else
                {
                    gameControl.control.SaveGame(1);
                    savecaution.SetActive(false);
                    if (pauseWindows[3].activeInHierarchy)
                        print("exitgame");
                }
            }
            else if (btn == "saveslot2")
            {
                if (File.Exists(Application.persistentDataPath + "/save2.dat") && lastButton != btn)
                {
                    savecaution.SetActive(true);
                }
                else
                {
                    gameControl.control.SaveGame(2);
                    savecaution.SetActive(false);
                    if (pauseWindows[3].activeInHierarchy)
                        print("exitgame");
                }
            }
            else if (btn == "saveslot3")
            {
                if (File.Exists(Application.persistentDataPath + "/save3.dat") && lastButton != btn)
                {
                    savecaution.SetActive(true);
                }
                else
                {
                    gameControl.control.SaveGame(3);
                    savecaution.SetActive(false);
                    if (pauseWindows[3].activeInHierarchy)
                        print("exitgame");
                }
            }
        }
        lastButton = btn;
    }

    void ClickAnswer()
    {
        string ans = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        for(int i = 0; i < npcDialogue[currentPage].player.Length; i++)
        {
            if (ans == npcDialogue[currentPage].player[i].answer)
            {
                dPl.SetActive(false);
                dNpc.SetActive(true);
                dNpc.transform.Find("txt").GetComponent<Text>().text = npcDialogue[currentPage].player[i].reactionToAnswer;
                npcDialogue[currentPage].player[i].consequenceToAnswer.Invoke();
            }
        }
        currentPage += 1;
        choice = false;
    }

    void ClickBook()
    {
        string b = EventSystem.current.currentSelectedGameObject.name;
        Book tBook = items.books.FirstOrDefault(i => i.id.ToString() == b);
        bookcase.transform.Find("bookText").gameObject.SetActive(true);
        bookReading = true;
        bookcase.transform.Find("bookText").GetChild(0).GetComponent<Text>().text = tBook.text;
    }

    void BtrToggle(bool toggleOn)
    {
        if (toggleOn)
        {
            gameControl.control.autoBattery = true;
            items.CheckBatteries();
            ShowEquips();
        }
        else
            gameControl.control.autoBattery = false;
    }

    void CloseAllPauseMenusExcept(int num)
    {
        for (int i = 0; i < pauseWindows.Length; i++)
        {
            if (i != num)
                pauseWindows[i].SetActive(false);
            else
                pauseWindows[i].SetActive(true);
        }

        if (num == 0)
            savecaution.SetActive(false);
    }

    public void ShowCollectedItem(string itemName)
    {
        showItemTimer = 0;
        newItem.SetActive(true);
        newItem.transform.Find("collectedName").GetComponent<Text>().text = itemName;
    }

    void LoadUI()
    {
        if (hp == null)
            hp = Instantiate(Resources.Load("ui/healthBar") as GameObject, canv, false);
        if (inv == null)
        {
            inv = Instantiate(Resources.Load("ui/inventory/inv") as GameObject, canv, false);
            itemContainer = inv.transform.Find("items").Find("itemInventory").gameObject;
            inv.transform.Find("active").Find("battery").Find("Toggle").GetComponent<Toggle>().isOn = gameControl.control.autoBattery;
            inv.transform.Find("active").Find("battery").Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(BtrToggle);
            inv.SetActive(false);
        }
        if (pausemenu == null)
        {
            pausemenu = Instantiate(Resources.Load("ui/pauseWindow") as GameObject, canv, false);
            pauseWindows = new GameObject[4];
            pauseWindows[0] = pausemenu.transform.Find("general").gameObject;
            pauseWindows[1] = pausemenu.transform.Find("options").gameObject;
            pauseWindows[2] = pausemenu.transform.Find("saves").gameObject;
            pauseWindows[3] = pausemenu.transform.Find("exit").gameObject;
            Button[] tempArray;
            tempArray = pausemenu.GetComponentsInChildren<Button>().ToArray();
            for (int i = 0; i < tempArray.Length; i++)
                tempArray[i].onClick.AddListener(PauseClickListener);

            savecaution = Instantiate(Resources.Load("ui/caution") as GameObject, pausemenu.transform, false);
            savecaution.SetActive(false);
            pausemenu.SetActive(false);
        }
        if (chest == null)
        {
            chest = Instantiate(Resources.Load("ui/inventory/chestWin") as GameObject, canv, false);
            chest_itemContainer = chest.transform.Find("items").GetChild(0).gameObject;
            chest_storedContainer = chest.transform.Find("stored").GetChild(0).gameObject;
            chest.SetActive(false);
        }
        if (merchant == null)
        {
            merchant = Instantiate(Resources.Load("ui/inventory/merchantWindow") as GameObject, canv, false);
            merchant_owned = merchant.transform.Find("items").GetChild(0).gameObject;
            merchant_selling = merchant.transform.Find("selling").GetChild(0).gameObject;
            merchant_playerMoney = merchant.transform.Find("playerMoney").GetComponent<Text>();
            merchant_playerMoney.text = gameControl.control.money.ToString();
            merchant.SetActive(false);
        }
        if (dBox == null)
        {
            dBox = Instantiate(Resources.Load("ui/dialoguebox") as GameObject, canv, false);
            dNpc = dBox.transform.Find("npc").gameObject;
            dPl = dBox.transform.Find("player").gameObject;
            //assign player sprite here
            answers = new GameObject[dPl.transform.Find("answerCont").transform.childCount];
            for (int i = 0; i < dPl.transform.Find("answerCont").transform.childCount; i++)
            {
                answers[i] = dPl.transform.Find("answerCont").transform.GetChild(i).gameObject;
                answers[i].GetComponent<Button>().onClick.AddListener(ClickAnswer);
            }

            dBox.SetActive(false);
        }
        if (bookcase == null)
        {
            bookcase = Instantiate(Resources.Load("ui/bookcase") as GameObject, canv, false);
            books = new GameObject[bookcase.transform.Find("bookContainer").transform.childCount];
            for(int i = 0; i < bookcase.transform.Find("bookContainer").transform.childCount; i++)
            {
                books[i] = bookcase.transform.Find("bookContainer").transform.GetChild(i).gameObject;
                books[i].GetComponent<Button>().onClick.AddListener(ClickBook);
                books[i].name = i.ToString();
                books[i].SetActive(false);
            }

            bookcase.transform.Find("addedbooks").gameObject.SetActive(false);
            bookcase.transform.Find("bookText").gameObject.SetActive(false);
            bookcase.SetActive(false);
        }
        if (newItem == null)
        {
            newItem = Instantiate(Resources.Load("ui/collected") as GameObject, canv, false);
            newItem.SetActive(false);
        }
    }

    //find children by name
    public Transform[] FindChildren(Transform tr, string name)
    {
        return tr.GetComponentsInChildren<Transform>().Where(t => t.name == name).ToArray();
    }
}