using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ui : MonoBehaviour {

    //W I P 

    Transform canv;
    GameObject inv, hp, pausemenu;
    GameObject itemContainer;
    KeyCode keyInventory, keyInteract;
    bool anyOpen;

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
                }

                if (inv.activeInHierarchy && Input.GetKeyDown(keyInventory))
                    Close(inv);
                break;
            case false:
                if (Input.GetKeyDown(keyInventory))
                    OpenInventory();
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
            foreach(Item2 i in items.ownedItems)
                if (!itemContainer.transform.Find(i.name))
                    AddItem(i, itemContainer);
        }

        CheckDuplicates(itemContainer, items.ownedItems);
    }

    void AddItem(Item2 itemToAdd, GameObject container)
    {
        GameObject i = Instantiate(Resources.Load("ui/inventory/item2") as GameObject, container.transform, false);
        i.name = itemToAdd.name + itemToAdd.id.ToString();
        i.transform.Find("name").GetComponent<Text>().text = itemToAdd.name;
    }

    void CheckDuplicates(GameObject g, List<Item2> l)
    {
        foreach(Transform child in g.transform)
        {
            Item2 t = l.FirstOrDefault(i => i.name + i.id.ToString() == child.name);
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
                List<Item2> tempList = l.FindAll(i => i.name + i.id.ToString() == child.name);
                string stacks = " (" + tempList.Count.ToString() + ")";
                child.transform.Find("name").GetComponent<Text>().text = t.name + stacks;
            }
            if (t == null)
                Destroy(child.gameObject);
        }
    }

    void Close(GameObject windowToClose)
    {
        windowToClose.SetActive(false);
        anyOpen = false;
        TogglePause();
    }

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
            pausemenu.SetActive(false);
        }
    }

    public Transform[] FindChildren(Transform tr, string name)
    {
        return tr.GetComponentsInChildren<Transform>().Where(t => t.name == name).ToArray();
    }
}
