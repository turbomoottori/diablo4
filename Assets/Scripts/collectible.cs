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
    public int gunAmmo;
    public string gunType, gunSpecial;
    public int bookId;
    public float batteryEnergy;
    [TextArea]
    public string bookText;
	
	void Start () {
        foreach(Collectibles c in gameControl.control.collectibles)
            if (c.cName == collectibleName && c.posX == transform.position.x && c.posZ == transform.position.z)
                Destroy(gameObject);
    }

    void AddItem()
    {
        switch (itemType)
        {
            case Type.Item:
                gameControl.invItems.Add(new Item()
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
                gameControl.invItems.Add(new Weapon()
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
                gameControl.invItems.Add(new Gun()
                {
                    name = collectibleName,
                    canSell = canSell,
                    value = value,
                    sellValue = sellValue,
                    weight = weight,
                    damage = weaponDamage,
                    speed = weaponSpeed,
                    range = gunRange,
                    rlspeed = gunReloadSpeed,
                    special = gunSpecial,
                    type = gunType
                });

                break;
            case Type.Book:
                gameControl.invItems.Add(new Book()
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
            case Type.Battery:
                GameObject globals = GameObject.Find("Globals");
                int newId;
                if (globals.GetComponent<battery>().allBatteries.Count == 0)
                    newId = 1;
                else
                    newId = globals.GetComponent<battery>().allBatteries.Count + 1;

                print(newId);
                gameControl.invItems.Add(new Battery()
                {
                    name = collectibleName + (batteryEnergy / 1 * 100).ToString("F0") + "%",
                    canSell = canSell,
                    id = newId,
                    energy = batteryEnergy,
                    isEmpty = false,
                    sellValue = sellValue,
                    stackable = stackable,
                    value = value,
                    weight = weight
                });
                break;
        }
    }

    public void PickUp()
    {
        gameObject.GetComponent<interactable>().HideE();
        gameControl.control.collectibles.Add(new Collectibles()
        {
            posX = transform.position.x,
            posZ=transform.position.z,
            cName = collectibleName
        });

        AddItem();
        menus.ShowCollected(collectibleName);

        if (itemType == Type.Battery)
            GameObject.Find("Globals").GetComponent<battery>().UpdateBatteryList();

        Destroy(this.gameObject);
    }

    public enum Type
    {
        Item,
        Weapon,
        Gun,
        Book,
        Battery
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


