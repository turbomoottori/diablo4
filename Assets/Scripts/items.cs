using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class items : MonoBehaviour {

    public static List<Item> ownedItems, storedItems;
    public static List<Book> books;
    public static Weapon equippedOne, equippedTwo;
    public static Battery inUse;
    public static int nextBatteryId;
    public static bool batteryOn;

	// Use this for initialization
	void Start () {
        if (ownedItems == null)
            ownedItems = new List<Item>();
        if (storedItems == null)
            storedItems = new List<Item>();
	}
	
	// Update is called once per frame
	void Update () {

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
