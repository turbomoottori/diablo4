using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class ui : MonoBehaviour {

    Transform canv;
    GameObject inv, hp, pausemenu, savecaution, chest, merchant, dBox, bookcase, newItem, popup, merchant_popup, tBox;
    GameObject itemContainer, chest_itemContainer, chest_storedContainer, merchant_owned, merchant_selling, dNpc, dPl;
    GameObject[] pauseWindows, answers, books;
    public static bool anyOpen;
    bool choice, bookReading, hoverOn, eHoverOn, changeKeys;
    string keyToChange, buttonName, lastButton = "";
    int tempInt = 0;
    public static GameObject interactableObject;
    public static List<Item> merchantItems;
    public static Dialogue[] currentConvo;
    public static int merchantAmmoB, merchantAmmoR, merchantAmmoS;
    int currentPage;
    float showItemTimer = 0f;
    public static float priceMultiplier;
    int tempHp, ammoBuyMode, tempAmmoAmount, finalValue;
    public static bool minigame = false;
    bool consequence = false;
    UnityEvent tempEvent;

	void Start () {
        canv = GameObject.Find("Canvas").transform;
        LoadUI();
        tempHp = gameControl.control.hp;
	}

    private void OnGUI()
    {
        if (changeKeys && Input.anyKeyDown)
        {
            if (Event.current.isKey)
                ChangeKeys(keyToChange, Event.current.keyCode);
            else if (Event.current.isMouse)
            {
                string mouseKeyText = "Mouse" + Event.current.button.ToString();
                KeyCode mouseKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), mouseKeyText);
                ChangeKeys(keyToChange, mouseKey);
            }
        }
    }

    void Update () {
        if (showItemTimer < 2f)
            showItemTimer += Time.deltaTime;
        if (showItemTimer > 2f || anyOpen)
            newItem.SetActive(false);

        if (gameControl.control.hp != tempHp)
        {
            hp.transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = (float)gameControl.control.hp / (float)gameControl.control.maxhp;
            tempHp = gameControl.control.hp;
        }

        if (hoverOn)
            HoverOnItem(buttonName, tempInt);
        if (eHoverOn)
            HoverEquip(tempInt);

        if (!minigame)
        {
            switch (anyOpen)
            {
                case true:
                    //closing active windows
                    if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(keys.savedKeys.inventoryKey))
                        Esc();

                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(keys.savedKeys.interactKey))
                    {
                        if (bookcase.activeInHierarchy)
                        {
                            if (bookcase.transform.Find("addedbooks").gameObject.activeInHierarchy)
                                bookcase.transform.Find("addedbooks").gameObject.SetActive(false);
                            if (bookReading)
                            {
                                bookcase.transform.Find("bookText").gameObject.SetActive(false);
                                bookReading = false;
                            }
                        }
                        if (dialogueui.isOpen && dialogueui.talker != 1)
                            SendMessage("CheckDialogue");
                        if (tBox.activeInHierarchy)
                            Close(tBox);
                    }
                    break;
                case false:
                    if (Input.GetKeyDown(keys.savedKeys.inventoryKey))
                        OpenInventory();
                    if (Input.GetKeyDown(KeyCode.Escape))
                        OpenPauseMenu();

                    //interacting
                    if (Input.GetKeyDown(keys.savedKeys.interactKey) && interactableObject != null)
                    {
                        switch (interactableObject.gameObject.GetComponent<interactable>().type)
                        {
                            case interactable.Type.merchant:
                                OpenMerchantWindow();
                                break;
                            case interactable.Type.collectible:
                                interactableObject.GetComponent<interactable>().HideE();
                                interactableObject.GetComponent<newCollectible>().PickUp();
                                break;
                            case interactable.Type.chest:
                                OpenChestWindow();
                                break;
                            case interactable.Type.npc:
                                interactableObject.GetComponent<dialogue_npc>().SendText();
                                SendMessage("CheckDialogue");
                                break;
                            case interactable.Type.bookcase:
                                OpenBookcase();
                                break;
                            case interactable.Type.door:
                                spawns.LoadLevel(interactableObject.GetComponent<interactable>().levelToLoad, interactableObject.GetComponent<interactable>().nextPositionName);
                                break;
                            case interactable.Type.deliveryLocation:
                                DeliveryQuest dq = quests.questList.FirstOrDefault(i => i.questName == interactableObject.GetComponent<interactable>().deliveryQuest) as DeliveryQuest;
                                for (int i = 0; i < dq.whereToDeliver.Length; i++)
                                {
                                    if (GameObject.Find(dq.whereToDeliver[i]) == interactableObject && dq.delivered[i] == false)
                                    {
                                        dq.delivered[i] = true;
                                        items.ownedItems.Remove(dq.itemToDeliver);
                                        ShowCollectedItem(dq.itemToDeliver.name + " delivered");
                                        interactableObject.GetComponent<interactable>().HideE();
                                        Destroy(interactableObject.GetComponent<interactable>());
                                    }
                                }
                                break;
                            case interactable.Type.workbench:
                                quests.workbenchUse();
                                switch (quests.workbenchStage)
                                {
                                    case 0:
                                        OpenThought("It's my workbench. I don't need it right now");
                                        break;
                                    case 1:
                                        //here we should have some animations or sounds or whatever
                                        //indicating that the player has built a dash backpack thing
                                        //also after the cutscene or whatever it's gonna be
                                        //you should remember to change the dash variable thing to true
                                        //because u know, it doesn't work if you don't do it
                                        break;
                                    case 2:
                                        OpenThought("You already know what tf this is and I'm damn sure I already used it so gtfo");
                                        break;
                                }
                                break;
                            case interactable.Type.inspectable:
                                OpenThought(interactableObject.GetComponent<interactable>().thought);
                                break;
                        }
                    }
                    break;
            }
        }
	}

    //check which window to close
    void Esc()
    {
        if (inv.activeInHierarchy)
            Close(inv);
        if (pauseWindows[1].activeInHierarchy || pauseWindows[2].activeInHierarchy || pauseWindows[3].activeInHierarchy)
            CloseAllPauseMenusExcept(0);
        if (pauseWindows[0].activeInHierarchy)
            Close(pausemenu);
        if (chest.activeInHierarchy)
            Close(chest);
        if (dBox.activeInHierarchy && !choice)
            Dialogue();
        if (merchant.activeInHierarchy && !merchant_popup.activeInHierarchy)
            Close(merchant);
        if (bookcase.activeInHierarchy && !bookReading && !bookcase.transform.Find("addedbooks").gameObject.activeInHierarchy)
            Close(bookcase);
        if (bookcase.transform.Find("addedbooks").gameObject.activeInHierarchy)
            bookcase.transform.Find("addedbooks").gameObject.SetActive(false);
        if (bookReading)
        {
            bookcase.transform.Find("bookText").gameObject.SetActive(false);
            bookReading = false;
        }
        if (merchant_popup.activeInHierarchy)
            merchant_popup.SetActive(false);
        if (pauseWindows[4].activeInHierarchy)
        {
            if (changeKeys)
                changeKeys = false;
            else
                CloseAllPauseMenusExcept(1);
        }
        if (tBox.activeInHierarchy)
            Close(tBox);
    }

    void OpenInventory()
    {
        anyOpen = true;
        inv.SetActive(true);
        TogglePause();
        ShowItems(itemContainer, items.ownedItems, buttonScript.buttonType.inventoryItem);
        CheckQuests();
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

    void OpenThought(string thought)
    {
        anyOpen = true;
        TogglePause();
        tBox.SetActive(true);
        tBox.transform.Find("Text").GetComponent<Text>().text = thought;
    }

    void Dialogue()
    {
        int maxPages = currentConvo.Length;

        if (currentPage < maxPages)
        {
            if (currentPage == 0)
            {
                anyOpen = true;
                dBox.SetActive(true);
                TogglePause();
                tempEvent = null;
            }

            //npc talks
            if (currentConvo[currentPage].who == who.npc)
            {
                dPl.SetActive(false);
                dNpc.SetActive(true);
                dNpc.transform.Find("txt").GetComponent<Text>().text = currentConvo[currentPage].npc.talk;

                if (currentConvo[currentPage].npc.consequenseWithoutAction != null)
                    currentConvo[currentPage].npc.consequenseWithoutAction.Invoke();

                currentPage += 1;
            }
            //player answers
            else
            {
                dPl.SetActive(true);
                dNpc.SetActive(false);
                choice = true;

                for (int i = 0; i < answers.Length; i++)
                {
                    if (i < currentConvo[currentPage].player.Length)
                    {
                        answers[i].SetActive(true);
                        answers[i].transform.GetChild(0).GetComponent<Text>().text = currentConvo[currentPage].player[i].answer;
                    }
                    else if (i >= currentConvo[currentPage].player.Length)
                        answers[i].SetActive(false);
                }
            }
        }
        //end dialogue
        else
        {
            if (tempEvent != null)
                tempEvent.Invoke();

            currentPage = 0;
            Close(dBox);
        }
    }

    //adds ammo as "item" in merchant's list
    void AddAmmoIcons(string ammoType)
    {
        GameObject i = Instantiate(Resources.Load("ui/inventory/item") as GameObject, merchant_selling.transform, false);
        i.name = "ammo_" + ammoType;
        i.GetComponent<buttonScript>().type = buttonScript.buttonType.ammo;
    }

    //adds items to lists
    void AddItem(Item itemToAdd, GameObject container, buttonScript.buttonType type)
    {
        GameObject i = Instantiate(Resources.Load("ui/inventory/item") as GameObject, container.transform, false);
        i.name = itemToAdd.name + itemToAdd.id.ToString();
        if (!itemToAdd.stackable)
            i.transform.Find("amount").gameObject.SetActive(false);
        i.GetComponent<buttonScript>().type = type;
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
                inv.transform.Find("quests").transform.Find("quest" + quest.questName).SetAsLastSibling();
            }

            if (quest.isMainQuest && !quest.completed)
                inv.transform.Find("quests").transform.Find("quest" + quest.questName).SetAsFirstSibling();
        }
    }

    //shows every item in list and checks duplicates
    void ShowItems(GameObject g, List<Item> l, buttonScript.buttonType type)
    {
        if (g == merchant_selling)
        {
            if (merchantAmmoB != 0)
                AddAmmoIcons("basic");
            if (merchantAmmoR != 0)
                AddAmmoIcons("rapid");
            if (merchantAmmoS != 0)
                AddAmmoIcons("shotgun");
        }

        if (l != null)
        {
            foreach (Item i in l)
                if (!g.transform.Find(i.name))
                    AddItem(i, g, type);
        }

        foreach(Transform child in g.transform)
        {
            Transform[] duplicateCheck = FindChildren(g.transform, child.name);
            if (duplicateCheck.Length > 1)
            {
                for (int i = 0; i < duplicateCheck.Length; i++)
                    if (i != 0)
                        Destroy(duplicateCheck[i].gameObject);
            }

            if (child.name.Contains("ammo_"))
            {
                switch (child.name)
                {
                    case "ammo_basic":
                        if (merchantAmmoB == 0)
                            Destroy(child.gameObject);
                        else
                            child.transform.Find("amount").GetComponent<Text>().text = merchantAmmoB.ToString();
                        break;
                    case "ammo_rapid":
                        if (merchantAmmoR == 0)
                            Destroy(child.gameObject);
                        else
                            child.transform.Find("amount").GetComponent<Text>().text = merchantAmmoR.ToString();
                        break;
                    case "ammo_shotgun":
                        if (merchantAmmoS == 0)
                            Destroy(child.gameObject);
                        else
                            child.transform.Find("amount").GetComponent<Text>().text = merchantAmmoS.ToString();
                        break;
                }
            }
            else
            {
                Item t = l.FirstOrDefault(i => i.name + i.id.ToString() == child.name);

                //display amount if item is stackable
                if (t != null && t.stackable)
                {
                    List<Item> tempList = l.FindAll(i => i.name + i.id.ToString() == child.name);
                    child.transform.Find("amount").GetComponent<Text>().text = tempList.Count.ToString();
                }

                if (t != null && (t == items.equippedOne || t == items.equippedTwo || t == items.inUse))
                    child.GetComponent<Image>().color = Color.gray;
                else
                    child.GetComponent<Image>().color = Color.white;

                if (t == null)
                    Destroy(child.gameObject);
            }
        }
    }

    //checks merchant prices and equips
    void CheckValuesAndEquips()
    {
        merchant.transform.Find("moneyammo").Find("playerMoney").Find("amount").GetComponent<Text>().text = gameControl.control.money.ToString();
        merchant.transform.Find("moneyammo").Find("basicAmmo").Find("amount").GetComponent<Text>().text = gameControl.basicAmmo.ToString();
        merchant.transform.Find("moneyammo").Find("rapidAmmo").Find("amount").GetComponent<Text>().text = gameControl.rapidAmmo.ToString();
        merchant.transform.Find("moneyammo").Find("shotgunAmmo").Find("amount").GetComponent<Text>().text = gameControl.shotgunAmmo.ToString();

        //changes color of the items that can't be sold
        foreach (Item i in items.ownedItems)
        {
            if (i == items.equippedOne || i == items.equippedTwo || i.questItem)
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

        //changes color of the items that can't be bought
        foreach (Item i in merchantItems)
        {
            if (NewValue(i) > gameControl.control.money)
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
        GameObject e1, e2, b, a1, a2, a3, m;
        e1 = inv.transform.Find("active").Find("equip1").gameObject;
        e2 = inv.transform.Find("active").Find("equip2").gameObject;
        b = inv.transform.Find("active").Find("battery").gameObject;
        m = inv.transform.Find("active").Find("playerMoney").GetChild(0).gameObject;
        a1 = inv.transform.Find("active").Find("ammoBasic").GetChild(0).gameObject;
        a2 = inv.transform.Find("active").Find("ammoRapid").GetChild(0).gameObject;
        a3 = inv.transform.Find("active").Find("ammoShotgun").GetChild(0).gameObject;

        m.GetComponent<Text>().text = gameControl.control.money.ToString();
        a1.GetComponent<Text>().text = gameControl.basicAmmo.ToString();
        a2.GetComponent<Text>().text = gameControl.rapidAmmo.ToString();
        a3.GetComponent<Text>().text = gameControl.shotgunAmmo.ToString();

        //displays equip 1
        if (items.equippedOne != null)
        {
            e1.transform.Find("title").GetComponent<Text>().text = items.equippedOne.name;
            if (items.equippedOne is Gun)
            {
                e1.transform.Find("ammo").gameObject.SetActive(true);
                Gun g = items.equippedOne as Gun;
                switch (g.type)
                {
                    case GunType.basic:
                        //change sprite
                        break;
                    case GunType.rapid:
                        //change sprite
                        break;
                    case GunType.shotgun:
                        //change sprite
                        break;
                }
            }
            else
                e1.transform.Find("ammo").gameObject.SetActive(false);
        }
        else
        {
            e1.transform.Find("title").GetComponent<Text>().text = "Empty";
            e1.transform.Find("ammo").gameObject.SetActive(false);
        }

        //displays equip 2
        if (items.equippedTwo != null)
        {
            e2.transform.Find("title").GetComponent<Text>().text = items.equippedTwo.name;
            if (items.equippedTwo is Gun)
            {
                e2.transform.Find("ammo").gameObject.SetActive(true);
                Gun g = items.equippedTwo as Gun;
                switch (g.type)
                {
                    case GunType.basic:
                        //change sprite
                        break;
                    case GunType.rapid:
                        //change sprite
                        break;
                    case GunType.shotgun:
                        //change sprite
                        break;
                }
            }
            else
                e2.transform.Find("ammo").gameObject.SetActive(false);
        }
        else
        {
            e2.transform.Find("title").GetComponent<Text>().text = "Empty";
            e2.transform.Find("ammo").gameObject.SetActive(false);
        }

        //displays battery in use
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
        StopHover();
        TogglePause();
    }

    //toggles pause
    public static void TogglePause()
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

    //clicking any item
    public void ClickedItem(bool leftClick, buttonScript.buttonType type, string name)
    {
        switch (type)
        {
            //item is in inventory
            case buttonScript.buttonType.inventoryItem:
                Item t = items.ownedItems.FirstOrDefault(i => i.name + i.id == name);
                if (t != null)
                {
                    //equipping a weapon
                    if(t is Weapon)
                    {
                        Weapon w = t as Weapon;
                        //equipping a weapon in slot 1
                        if (leftClick)
                        {
                            //some item is already equipped on slot 1
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
                        //equipping a weapon in slot 2
                        else if (!leftClick)
                        {
                            //ome item is already equipped in slot 2
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
                    //equipping a battery
                    else if(t is Battery)
                    {
                        Battery b = t as Battery;
                        if (!b.isEmpty)
                        {
                            if (items.inUse != null)
                            {
                                if (items.inUse != b)
                                    items.inUse = b;
                                else
                                    print("this battery is already in use");
                            }
                            else
                                items.inUse = b;
                        }
                        else
                            print("this battery is empty");
                    }
                    else if(t is Consumable)
                    {
                        Consumable c = t as Consumable;
                        if (gameControl.control.hp + c.healAmount <= gameControl.control.maxhp)
                            gameControl.control.hp += c.healAmount;
                        else if (gameControl.control.hp + c.healAmount > gameControl.control.maxhp)
                            gameControl.control.hp = gameControl.control.maxhp;

                        StopHover();
                        items.ownedItems.Remove(c);
                    }
                }
                ShowItems(itemContainer, items.ownedItems, buttonScript.buttonType.inventoryItem);
                ShowEquips();
                break;

            //buy item
            case buttonScript.buttonType.readyToBuy:
                StopHover();
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
                ShowItems(merchant_owned, items.ownedItems, buttonScript.buttonType.readyToSell);
                ShowItems(merchant_selling, merchantItems, buttonScript.buttonType.readyToBuy);
                CheckValuesAndEquips();
                break;

            //sell item
            case buttonScript.buttonType.readyToSell:
                StopHover();
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

            //can't buy item
            case buttonScript.buttonType.cantBuy:
                print("can't buy");
                break;

            //can't sell item
            case buttonScript.buttonType.cantSell:
                print("can't sell");
                break;

            //store item in chest
            case buttonScript.buttonType.readyToStore:
                tempItem = items.ownedItems.FirstOrDefault(i => i.name + i.id == name);
                if (tempItem != null && tempItem != items.equippedOne && tempItem != items.equippedTwo && tempItem != items.inUse)
                {
                    StopHover();
                    items.ownedItems.Remove(tempItem);
                    items.storedItems.Add(tempItem);
                    ShowItems(chest_itemContainer, items.ownedItems, buttonScript.buttonType.readyToStore);
                    ShowItems(chest_storedContainer, items.storedItems, buttonScript.buttonType.readyToTake);
                }
                break;

            //take item from chest
            case buttonScript.buttonType.readyToTake:
                StopHover();
                tempItem = items.storedItems.FirstOrDefault(i => i.name + i.id == name);
                if (tempItem != null)
                {
                    items.storedItems.Remove(tempItem);
                    items.ownedItems.Add(tempItem);
                    ShowItems(chest_itemContainer, items.ownedItems, buttonScript.buttonType.readyToStore);
                    ShowItems(chest_storedContainer, items.storedItems, buttonScript.buttonType.readyToTake);
                }
                break;

            //buy ammo
            case buttonScript.buttonType.ammo:
                StopHover();
                merchant_popup.SetActive(true);
                tempAmmoAmount = 0;

                if (name == "ammo_basic")
                    ammoBuyMode = 1;
                else if (name == "ammo_rapid")
                    ammoBuyMode = 2;
                else
                    ammoBuyMode = 3;

                merchant_popup.transform.Find("container").Find("amount").GetComponent<Text>().text = "0 (costs 0)";
                break;
        }
    }

    //click a button in pause menu
    void PauseClickListener()
    {
        string btn = EventSystem.current.currentSelectedGameObject.name;

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
        else if (btn == "keybinds")
            CloseAllPauseMenusExcept(4);
        else if (btn == "backButton")
            CloseAllPauseMenusExcept(0);
        else if (btn.Contains("changekey_"))
        {
            string end = btn.Replace("changekey_", "");
            GameObject.Find(btn).GetComponent<Button>().image.color = Color.gray;
            changeKeys = true;
            keyToChange = end;
        }
        else if (btn == "defaults")
        {
            keys.DefaultKeys();
            ShowKeyBinds();
        }
        else if (btn.Contains("saveslot"))
        {
            int num = int.Parse(btn.Replace("saveslot", ""));
            if (File.Exists(Application.persistentDataPath + "/save" + num.ToString() + ".dat") && lastButton != btn)
            {
                savecaution.SetActive(true);
            }
            else
            {
                gameControl.control.SaveGame(num);
                savecaution.SetActive(false);
                if (pauseWindows[3].activeInHierarchy)
                    print("exítgame");
            }
        }
        lastButton = btn;
    }

    //change keybinds
    void ChangeKeys(string keyToChange, KeyCode newKey)
    {
        GameObject.Find("changekey_" + keyToChange).GetComponent<Button>().image.color = Color.white;

        if (keys.AcceptNewKey(newKey))
        {
            switch (keyToChange)
            {
                case "inventory":
                    keys.savedKeys.inventoryKey = newKey;
                    break;
                case "interact":
                    keys.savedKeys.interactKey = newKey;
                    break;
                case "dash":
                    keys.savedKeys.dashKey = newKey;
                    break;
                case "slowtime":
                    keys.savedKeys.slowtimeKey = newKey;
                    break;
                case "attack":
                    keys.savedKeys.attackKey = newKey;
                    break;
                case "spAttack":
                    keys.savedKeys.spAttackKey = newKey;
                    break;
            }
            pauseWindows[4].transform.Find("changekey_" + keyToChange).Find("keycode").GetComponent<Text>().text = newKey.ToString();
        }

        if(newKey != KeyCode.Escape)
            changeKeys = false;
    }

    //choose an answer in dialogue
    void ClickAnswer()
    {
        string ans = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        for(int i = 0; i < currentConvo[currentPage].player.Length; i++)
        {
            if (ans == currentConvo[currentPage].player[i].answer)
            {
                dPl.SetActive(false);
                dNpc.SetActive(true);
                dNpc.transform.Find("txt").GetComponent<Text>().text = currentConvo[currentPage].player[i].reactionToAnswer;
                tempEvent = currentConvo[currentPage].player[i].consequenceToAnswer;
                //currentConvo[currentPage].player[i].consequenceToAnswer.Invoke();
            }
        }
        currentPage += 1;
        choice = false;
    }

    //open a book
    void ClickBook()
    {
        string b = EventSystem.current.currentSelectedGameObject.name;
        Book tBook = items.books.FirstOrDefault(i => i.id.ToString() == b);
        bookcase.transform.Find("bookText").gameObject.SetActive(true);
        bookReading = true;
        bookcase.transform.Find("bookText").GetChild(0).GetComponent<Text>().text = tBook.text;
    }

    //unequip a weapon or battery
    public void ClickEquip(int equipNum)
    {
        switch (equipNum)
        {
            case 1:
                items.equippedOne = null;
                break;
            case 2:
                items.equippedTwo = null;
                break;
            case 3:
                items.inUse = null;
                break;
        }
        ShowItems(itemContainer, items.ownedItems, buttonScript.buttonType.inventoryItem);
        ShowEquips();
    }

    //buying ammo
    public void MerchantAmmoClicks()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        int maxAmount = 0;
        int ammoBaseValue = 0;

        switch (ammoBuyMode)
        {
            case 1:
                maxAmount = merchantAmmoB;
                ammoBaseValue = items.ammoValueB;
                break;
            case 2:
                maxAmount = merchantAmmoR;
                ammoBaseValue = items.ammoValueR;
                break;
            case 3:
                maxAmount = merchantAmmoS;
                ammoBaseValue = items.ammoValueS;
                break;
        }

        switch (buttonName)
        {
            case "more":
                if (tempAmmoAmount < maxAmount && CanAffordNextAmmo(ammoBuyMode))
                    tempAmmoAmount += 1;
                break;
            case "less":
                if (tempAmmoAmount > 0)
                    tempAmmoAmount -= 1;
                break;
            case "confirm":
                if (ammoBuyMode == 1)
                {
                    merchantAmmoB -= tempAmmoAmount;
                    gameControl.basicAmmo += tempAmmoAmount;
                    gameControl.control.money -= finalValue;
                    merchant_popup.SetActive(false);
                    ShowItems(merchant_selling, merchantItems, buttonScript.buttonType.readyToBuy);
                    CheckValuesAndEquips();
                    interactableObject.GetComponent<merchant>().basicAmmo = merchantAmmoB;
                    interactableObject.GetComponent<merchant>().rapidAmmo = merchantAmmoR;
                    interactableObject.GetComponent<merchant>().shotgunAmmo = merchantAmmoS;
                    interactableObject.GetComponent<merchant>().SaveAmmoAmont();
                }
                break;
        }

        ammoBaseValue = (int)(ammoBaseValue * priceMultiplier);
        finalValue = ammoBaseValue * tempAmmoAmount;
        merchant_popup.transform.Find("container").Find("amount").GetComponent<Text>().text = tempAmmoAmount.ToString() + " (costs " + finalValue.ToString() + ")";
    }

    //checks if the next piece of ammo is affordable or not
    public bool CanAffordNextAmmo(int ammoType)
    {
        int newValue, nextValue = 0;

        switch (ammoType)
        {
            case 1:
                newValue = (int)(items.ammoValueB * priceMultiplier);
                nextValue = newValue * (tempAmmoAmount + 1);
                break;
            case 2:
                newValue = (int)(items.ammoValueR * priceMultiplier);
                nextValue = newValue * (tempAmmoAmount + 1);
                break;
            case 3:
                newValue = (int)(items.ammoValueS * priceMultiplier);
                nextValue = newValue * (tempAmmoAmount + 1);
                break;
        }

        if (nextValue > gameControl.control.money)
            return false;
        else
            return true;
    }

    //toggle auto battery
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

    //displays key binds
    void ShowKeyBinds()
    {
        foreach(Transform t in pauseWindows[4].transform)
        {
            if (t.name.Contains("changekey_"))
            {
                string keyToCheck = t.name.Replace("changekey_", "");
                switch (keyToCheck)
                {
                    case "inventory":
                        t.Find("keycode").GetComponent<Text>().text = keys.savedKeys.inventoryKey.ToString();
                        break;
                    case "interact":
                        t.Find("keycode").GetComponent<Text>().text = keys.savedKeys.interactKey.ToString();
                        break;
                    case "dash":
                        t.Find("keycode").GetComponent<Text>().text = keys.savedKeys.dashKey.ToString();
                        break;
                    case "slowtime":
                        t.Find("keycode").GetComponent<Text>().text = keys.savedKeys.slowtimeKey.ToString();
                        break;
                    case "attack":
                        t.Find("keycode").GetComponent<Text>().text = keys.savedKeys.attackKey.ToString();
                        break;
                    case "spAttack":
                        t.Find("keycode").GetComponent<Text>().text = keys.savedKeys.spAttackKey.ToString();
                        break;
                }
                t.GetComponent<Button>().image.color = Color.white;
            }
        }
    }

    //shows item info on popup
    public void HoverOnItem(string btnName, int listNum)
    {
        buttonName = btnName;
        tempInt = listNum;
        hoverOn = true;
        Item hoveredItem = new Item();

        switch (listNum)
        {
            case 1:
                hoveredItem = items.ownedItems.FirstOrDefault(i => i.name + i.id == btnName);
                break;
            case 2:
                hoveredItem = items.storedItems.FirstOrDefault(i => i.name + i.id == btnName);
                break;
            case 3:
                hoveredItem = merchantItems.FirstOrDefault(i => i.name + i.id == btnName);
                break;
        }

        popup.SetActive(true);
        if (listNum == 3)
            popup.transform.Find("valwt").GetComponent<Text>().text = "cost " + NewValue(hoveredItem).ToString() + "    wt." + hoveredItem.weight.ToString();
        else 
            popup.transform.Find("valwt").GetComponent<Text>().text = "value " + hoveredItem.baseValue.ToString() + "    wt." + hoveredItem.weight.ToString();

        if (hoveredItem is Battery)
        {
            Battery b = hoveredItem as Battery;
            popup.transform.Find("name").GetComponent<Text>().text = "Battery " + ((b.energy / 1) * 100).ToString("F0") + "%";
        }
        else
            popup.transform.Find("name").GetComponent<Text>().text = hoveredItem.name;

        Vector3 offset = new Vector3(-50, 25, 0);
        Vector3 newPos = Input.mousePosition + offset;
        popup.transform.position = newPos;
    }

    //shows equipped item's info
    public void HoverEquip(int equipNum)
    {
        switch (equipNum)
        {
            case 1:
                if (items.equippedOne != null)
                {
                    popup.SetActive(true);
                    eHoverOn = true;
                    tempInt = equipNum;
                    popup.transform.Find("name").GetComponent<Text>().text = items.equippedOne.name;
                    popup.transform.Find("valwt").GetComponent<Text>().text = "value " + items.equippedOne.baseValue.ToString() + "    wt." + items.equippedOne.weight.ToString();
                }
                break;
            case 2:
                if (items.equippedTwo != null)
                {
                    popup.SetActive(true);
                    eHoverOn = true;
                    tempInt = equipNum;
                    popup.transform.Find("name").GetComponent<Text>().text = items.equippedTwo.name;
                    popup.transform.Find("valwt").GetComponent<Text>().text = "value " + items.equippedTwo.baseValue.ToString() + "    wt." + items.equippedTwo.weight.ToString();
                }
                break;
            case 3:
                if (items.inUse != null)
                {
                    popup.SetActive(true);
                    eHoverOn = true;
                    tempInt = equipNum;
                    popup.transform.Find("name").GetComponent<Text>().text = "Battery " + ((items.inUse.energy / 1) * 100).ToString("F0") + "%";
                    popup.transform.Find("valwt").GetComponent<Text>().text = "value " + items.inUse.baseValue.ToString() + "    wt." + items.inUse.weight.ToString();
                }
                break;
        }

        Vector3 offset = new Vector3(-50, 25, 0);
        Vector3 newPos = Input.mousePosition + offset;
        popup.transform.position = newPos;
    }

    //closes popup window
    public void StopHover()
    {
        hoverOn = false;
        eHoverOn = false;
        popup.SetActive(false);
    }

    //controls pause menu
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

        if (num == 1)
            gameControl.control.SaveOptions();

        if (num == 2)
        {
            if (File.Exists(Application.persistentDataPath + "/save1.dat"))
                pauseWindows[2].transform.Find("saveslot1").GetComponent<Button>().image.color = Color.gray;
            else
                pauseWindows[2].transform.Find("saveslot1").GetComponent<Button>().image.color = Color.white;

            if (File.Exists(Application.persistentDataPath + "/save2.dat"))
                pauseWindows[2].transform.Find("saveslot2").GetComponent<Button>().image.color = Color.gray;
            else
                pauseWindows[2].transform.Find("saveslot2").GetComponent<Button>().image.color = Color.white;

            if (File.Exists(Application.persistentDataPath + "/save3.dat"))
                pauseWindows[2].transform.Find("saveslot3").GetComponent<Button>().image.color = Color.gray;
            else
                pauseWindows[2].transform.Find("saveslot3").GetComponent<Button>().image.color = Color.white;
        }

        if (num == 4)
            ShowKeyBinds();
    }

    //displays name and image of recently collected item
    public void ShowCollectedItem(string itemName)
    {
        showItemTimer = 0;
        newItem.SetActive(true);
        newItem.transform.Find("collectedName").GetComponent<Text>().text = itemName;
    }

    //loads every element in ui
    void LoadUI()
    {
        if (hp == null)
            hp = Instantiate(Resources.Load("ui/healthBar") as GameObject, canv, false);
        if (tBox == null)
        {
            tBox = Instantiate(Resources.Load("ui/thoughtbox") as GameObject, canv, false);
            tBox.SetActive(false);
        }
        if (inv == null)
        {
            inv = Instantiate(Resources.Load("ui/inventory/inv") as GameObject, canv, false);
            itemContainer = inv.transform.Find("items").Find("itemInventory").gameObject;
            inv.transform.Find("active").Find("battery").Find("Toggle").GetComponent<Toggle>().isOn = gameControl.control.autoBattery;
            inv.transform.Find("active").Find("battery").Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(BtrToggle);
            inv.SetActive(false);
            popup = Instantiate(Resources.Load("ui/inventory/popup") as GameObject, canv, false);
            popup.SetActive(false);
        }
        if (pausemenu == null)
        {
            pausemenu = Instantiate(Resources.Load("ui/pauseWindow") as GameObject, canv, false);
            pauseWindows = new GameObject[5];
            pauseWindows[0] = pausemenu.transform.Find("general").gameObject;
            pauseWindows[1] = pausemenu.transform.Find("options").gameObject;
            pauseWindows[2] = pausemenu.transform.Find("saves").gameObject;
            pauseWindows[3] = pausemenu.transform.Find("exit").gameObject;
            pauseWindows[4] = pausemenu.transform.Find("keys").gameObject;
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
            merchant.transform.Find("moneyammo").Find("playerMoney").Find("amount").GetComponent<Text>().text = gameControl.control.money.ToString();
            merchant_popup = merchant.transform.Find("howManyWindow").gameObject;
            merchant_popup.transform.Find("container").Find("more").GetComponent<Button>().onClick.AddListener(MerchantAmmoClicks);
            merchant_popup.transform.Find("container").Find("less").GetComponent<Button>().onClick.AddListener(MerchantAmmoClicks);
            merchant_popup.transform.Find("container").Find("confirm").GetComponent<Button>().onClick.AddListener(MerchantAmmoClicks);
            merchant_popup.SetActive(false);
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
        popup.transform.SetAsLastSibling();
    }

    //calculates the price merchant will sell an item
    public int NewValue(Item item)
    {
        float tempValue = item.baseValue * priceMultiplier;
        return (int)tempValue;
    }

    //find children by name
    public Transform[] FindChildren(Transform tr, string name)
    {
        return tr.GetComponentsInChildren<Transform>().Where(t => t.name == name).ToArray();
    }
}
