using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class npcSpeechSimple : MonoBehaviour
{
    //only simple dialogue stuff for merchants, chests and regular npcs

    GameObject globals, e;
    public bool wantsToTalk = true;
    public interactType type;
    bool isClose = false;

    public Speak[] talks;

    public List<Item> items;
    public List<Weapon> swords;
    public List<Gun> guns;

    void Start()
    {
        globals = GameObject.Find("Globals");

        e = Instantiate(Resources.Load("ui/interact", typeof(GameObject))) as GameObject;
        e.transform.SetParent(GameObject.Find("Canvas").transform, false);
        e.SetActive(false);

        if (type == interactType.merchant)
        {
            MerchantData m = gameControl.control.merchs.FirstOrDefault(i => i.merchantName == gameObject.name);

            //load merchant's items
            if (m != null)
            {
                items = new List<Item>();
                items = m.merchantItems;
            }
            else if (m == null)
            {
                foreach (Weapon w in swords)
                    items.Add(w);

                foreach (Gun g in guns)
                    items.Add(g);

                foreach (Item i in menus.invItems)
                    CheckItemDuplicates(i.name);

                gameControl.control.merchs.Add(new MerchantData()
                {
                    merchantName = gameObject.name,
                    merchantItems = items
                });
            }
        }
    }

    //removes items player already owns
    void CheckItemDuplicates(string name)
    {
        Item temp = items.FirstOrDefault(i => i.name == name);
        if (temp != null && !temp.stackable)
            items.Remove(temp);
    }

    void Update()
    {
        e.transform.position = Camera.main.WorldToScreenPoint(transform.position);

        switch (type)
        {
            case interactType.chest:
                //player is close enough to interact
                if (isClose)
                {
                    if (Input.GetKeyDown(KeyCode.E) && !menus.anyOpen)
                    {
                        menus.chestClose = true;
                    }
                }
                break;
            case interactType.merchant:
                //player is close enough to interact
                if (isClose)
                {
                    globals.GetComponent<menus>().ChangeMerchantItems(items);
                    if (Input.GetKeyDown(KeyCode.E) && !menus.anyOpen)
                    {
                        menus.merchClose = true;
                    }
                }
                break;
            case interactType.regular:
                if (wantsToTalk && isClose)
                {

                    if (Input.GetKeyDown(KeyCode.E) && !menus.anyOpen)
                    {
                        menus.tempSpeak = talks;
                        menus.talkReady = true;
                    }
                }
                break;
        }

    }

    public enum interactType
    {
        chest,
        merchant,
        regular
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "interactzone")
        {
            e.SetActive(true);
            isClose = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "interactzone")
        {
            e.SetActive(false);
            isClose = false;
        }

    }
}
