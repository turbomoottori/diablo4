using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class items : MonoBehaviour {

    // W I P 

    public static List<Item2> ownedItems, storedItems;
    public static List<Book2> books;
    public static Weapon equippedOne, equippedTwo;
    public static Battery2 inUse;
    public static int nextBatteryId;
    public static bool batteryOn;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
}

[System.Serializable]
public class Item2
{
    public string name;
    public int id, baseValue, weight;
    public bool stackable;
}

[System.Serializable]
public class Weapon2 : Item2
{
    public int damage;
    public float speed;
}

[System.Serializable]
public class Gun2 : Weapon2
{
    public GunType type;
    public float range;
}

[System.Serializable]
public class Book2 : Item2
{
    [TextArea]
    public string text;
}

[System.Serializable]
public class Battery2 : Item2
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
