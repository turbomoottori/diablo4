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
    float energyDrain = 0.2f;
    public static List<Battery> batteriesToUse;
    public static int ammoValueB = 2;
    public static int ammoValueR = 1;
    public static int ammoValueS = 3;
    int itemAmount = 0;
    public static int carrywt;

	void Start () {
        if (ownedItems == null)
            ownedItems = new List<Item>();
        if (storedItems == null)
            storedItems = new List<Item>();
        if (books == null)
            books = new List<Book>();
        if (batteriesToUse == null)
            batteriesToUse = new List<Battery>();
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
                    CheckBatteries();
                else
                {
                    inUse.energy = 0;
                    inUse = null;
                }
            }
        }

        if (ownedItems.Count != itemAmount)
            ChangeCarryWeight();
    }

    void ChangeCarryWeight()
    {
        itemAmount = ownedItems.Count;
        carrywt = 0;
        foreach(Item i in ownedItems)
            carrywt += i.weight;

        float pr = (float)carrywt / 100f;
        playerMovement.speedmodifier = pr;
    }

    public static void NewBattery()
    {
        if (batteriesToUse.Count > 0)
            inUse = batteriesToUse.FirstOrDefault();
    }

    public static void CheckBatteries()
    {
        foreach(Item i in ownedItems)
        {
            if(i is Battery)
            {
                Battery b = i as Battery;
                if (!b.isEmpty && !batteriesToUse.Exists(battery => battery == b))
                    batteriesToUse.Add(b);
            }
        }

        batteriesToUse.RemoveAll(b => b.isEmpty == true);
        if (batteriesToUse.Count <= 0)
            inUse = null;
        if (batteriesToUse.Count > 0 && gameControl.control.autoBattery)
            NewBattery();
    }

    public static bool batteryOn()
    {
        if (inUse == null)
            return false;
        else
            return true;
    }
}

[System.Serializable]
public class Item
{
    public string name;
    public int id, baseValue, weight;
    public bool stackable, questItem;
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

[System.Serializable]
public class Consumable : Item
{
    public int healAmount;
}

public enum GunType
{
    basic,
    shotgun,
    rapid
}
