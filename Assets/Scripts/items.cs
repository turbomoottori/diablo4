using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class items : MonoBehaviour {

    public static List<Item> ownedItems, storedItems;
    public static List<Book> books;
    public static Weapon equippedOne, equippedTwo;
    public static Battery inUse;
    public static int nextBatteryId;
    public static bool batteryOn;
    float energyDrain = 0.2f;
    List<Battery> batteriesToUse;

	void Start () {
        if (ownedItems == null)
            ownedItems = new List<Item>();
        if (storedItems == null)
            storedItems = new List<Item>();
        if (books == null)
            books = new List<Book>();
	}

    void Update()
    {
        if (inUse != null)
        {
            inUse.energy -= energyDrain * Time.deltaTime;
            if (inUse.energy <= 0)
            {
                inUse.isEmpty = true;
                if (gameControl.control.autoBattery)
                    NewBattery();
            }
        }
    }

    void NewBattery()
    {
        CheckBatteries();
        if (batteriesToUse.Count > 0)
            inUse = batteriesToUse.FirstOrDefault();
    }

    void CheckBatteries()
    {
        foreach(Item i in ownedItems)
        {
            if(i is Battery)
            {
                Battery b = i as Battery;
                if (!b.isEmpty)
                    batteriesToUse.Add(b);
            }
        }

        batteriesToUse.RemoveAll(b => b.isEmpty == true);
    }
}

[System.Serializable]
public class Item
{
    public string name;
    public int id, baseValue, weight;
    public bool stackable;
}

[System.Serializable]
public class Weapon : Item
{
    public int damage;
    public float speed;
}

[System.Serializable]
public class Gun : Weapon
{
    public GunType type;
    public float range;
}

[System.Serializable]
public class Book : Item
{
    [TextArea]
    public string text;
}

[System.Serializable]
public class Battery : Item
{
    public float energy;
    public bool isEmpty;
}

public enum GunType
{
    basic,
    shotgun,
    rapid
}
