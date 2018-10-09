using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class collectible : MonoBehaviour {

    public string collectibleName;
    public int value, sellValue, weight;
    public bool stackable, canSell;
    public Type itemType;
    public int weaponDamage;
    public float weaponSpeed, gunReloadSpeed, gunRange;
    public int gunBullets;
    public string gunType, gunSpecial;
    public int bookId;
    [TextArea]
    public string bookText;
    GameObject e;
    bool isClose = false;
	
	void Start () {
        e = Instantiate(Resources.Load("ui/interact", typeof(GameObject))) as GameObject;
        e.transform.SetParent(GameObject.Find("Canvas").transform, false);
        e.SetActive(false);

        foreach(Collectibles c in gameControl.control.collectibles)
            if (c.cName == collectibleName && c.posX == transform.position.x && c.posZ == transform.position.z)
                Destroy(gameObject);
    }

    void Update()
    {
        e.transform.position = Camera.main.WorldToScreenPoint(transform.position);

        if (isClose && Input.GetKeyDown(KeyCode.E))
            PickUp();
    }

    void AddItem()
    {
        switch (itemType)
        {
            case Type.Item:
                menus.invItems.Add(new Item()
                {
                    name = collectibleName,
                    value = value,
                    sellValue = sellValue,
                    weight = weight,
                    canSell = canSell,
                    stackable = stackable
                });

                break;
            case Type.Weapon:
                menus.invItems.Add(new Weapon()
                {
                    name = collectibleName,
                    canSell = canSell,
                    value = value,
                    sellValue = sellValue,
                    weight = weight,
                    damage = weaponDamage,
                    speed = weaponSpeed
                });

                break;
            case Type.Gun:
                menus.invItems.Add(new Gun()
                {
                    name = collectibleName,
                    canSell = canSell,
                    value = value,
                    sellValue = sellValue,
                    weight = weight,
                    damage = weaponDamage,
                    speed = weaponSpeed,
                    bullets = gunBullets,
                    range = gunRange,
                    rlspeed = gunReloadSpeed,
                    special = gunSpecial,
                    type = gunType
                });

                break;
            case Type.Book:
                menus.invItems.Add(new Book()
                {
                    name = collectibleName,
                    canSell = canSell,
                    id = bookId,
                    sellValue = sellValue,
                    stackable = stackable,
                    txt = bookText,
                    value = value,
                    weight = weight
                });

                break;
        }
    }

    void PickUp()
    {
        e.SetActive(false);

        gameControl.control.collectibles.Add(new Collectibles()
        {
            posX = transform.position.x,
            posZ=transform.position.z,
            cName = collectibleName
        });

        AddItem();
        menus.ShowCollected(collectibleName);
        Destroy(this.gameObject);
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

    public enum Type
    {
        Item,
        Weapon,
        Gun,
        Book
    }

    float Distance(Vector3 st, Vector3 en)
    {
        float d = 0.0f;

        float dx = st.x - en.x;
        float dy = st.y - en.y;
        float dz = st.z - en.z;

        d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);


        return d;
    }
}


