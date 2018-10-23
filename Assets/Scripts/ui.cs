using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ui : MonoBehaviour {

    //W I P 

    Transform canv;
    GameObject inv, hp, pausemenu, savecaution, chest;
    GameObject itemContainer, chest_itemContainer, chest_storedContainer;
    GameObject[] pauseWindows;
    KeyCode keyInventory, keyInteract;
    bool anyOpen;
    public static string closestInteractable;
    public static GameObject interactableObject;
    public static List<Item> merchantItems;

	void Start () {
        canv = GameObject.Find("Canvas").transform;
        DefaultKeys();
        LoadUI();
	}

    //sets default keys
    void DefaultKeys()
    {
        keyInventory = KeyCode.Tab;
        keyInteract = KeyCode.E;
    }
	
	void Update () {
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
                }

                if (inv.activeInHierarchy && Input.GetKeyDown(keyInventory))
                    Close(inv);
                break;
            case false:
                if (Input.GetKeyDown(keyInventory))
                    OpenInventory();
                if (Input.GetKeyDown(KeyCode.Escape))
                    OpenPauseMenu();
                if (Input.GetKeyDown(keyInteract))
                {
                    switch (closestInteractable)
                    {
                        case "merchant":
                            print("open merchant stuff");
                            break;
                        case "collectible":
                            interactableObject.GetComponent<newCollectible>().PickUp();
                            break;
                        case "chest":
                            OpenChestWindow();
                            break;
                        case "bookcase":
                            print("open bookcase");
                            break;
                        case "npc":
                            interactableObject.GetComponent<npc>().Interact();
                            break;
                        case "nobody":
                            print("nobody to interact");
                            break;
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

        //lists all items
        if (items.ownedItems != null)
        {
            foreach(Item i in items.ownedItems)
                if (!itemContainer.transform.Find(i.name))
                    AddItem(i, itemContainer);
        }

        CheckDuplicates(itemContainer, items.ownedItems);
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

        //displays owned items
        if (items.ownedItems != null)
        {
            foreach (Item i in items.ownedItems)
                if (!chest_itemContainer.transform.Find(i.name))
                    AddItem(i, chest_itemContainer);
        }
        CheckDuplicates(chest_itemContainer, items.ownedItems);

        //displays stored items
        if(items.storedItems!=null)
        {
            foreach (Item j in items.storedItems)
                if (!chest_storedContainer.transform.Find(j.name))
                    AddItem(j, chest_storedContainer);
        }
        CheckDuplicates(chest_storedContainer, items.storedItems);
    }

    //adds items to lists
    void AddItem(Item itemToAdd, GameObject container)
    {
        GameObject i = Instantiate(Resources.Load("ui/inventory/item2") as GameObject, container.transform, false);
        i.name = itemToAdd.name + itemToAdd.id.ToString();
        i.transform.Find("name").GetComponent<Text>().text = itemToAdd.name;
    }

    //checks duplicates from list
    void CheckDuplicates(GameObject g, List<Item> l)
    {
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

    void LoadUI()
    {
        if (hp == null)
            hp = Instantiate(Resources.Load("ui/healthBar") as GameObject, canv, false);
        if (inv == null)
        {
            inv = Instantiate(Resources.Load("ui/inventory/inv") as GameObject, canv, false);
            itemContainer = inv.transform.Find("items").Find("itemInventory").gameObject;
            inv.SetActive(false);
        }
        if(pausemenu == null)
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
        if(chest == null)
        {
            chest = Instantiate(Resources.Load("ui/inventory/chestWin") as GameObject, canv, false);
            chest_itemContainer = chest.transform.Find("items").GetChild(0).gameObject;
            chest_storedContainer = chest.transform.Find("stored").GetChild(0).gameObject;
            chest.SetActive(false);
        }
    }

    //find children by name
    public Transform[] FindChildren(Transform tr, string name)
    {
        return tr.GetComponentsInChildren<Transform>().Where(t => t.name == name).ToArray();
    }
}
