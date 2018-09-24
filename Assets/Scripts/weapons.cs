using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapons : MonoBehaviour {

    public static weapons w;

    public static int damage1, damage2;
    public static float speed1, speed2;
    public static int bullets1, bullets2;
    public static float range1, range2, rlspeed1, rlspeed2;

    //weapon type can be "normal", "rapid" or "shotgun"
    //weapon's special can be "big", "unlimited" or "multi"
    public static string type1, type2, special1, special2;

    //0 is no weapon, 1 is sword, 2 is gun
    public static int weaponType1, weaponType2;

    void Awake()
    {
        if (w == null)
        {
            DontDestroyOnLoad(gameObject);
            w = this;
        }
        else
        {
            Destroy(gameObject);
        }

        LoadEquips();
    }

    public void LoadEquips()
    {
        foreach(Item i in menus.invItems)
        {
            if (i is Weapon)
            {
                Weapon w = i as Weapon;
                if (w.name == menus.equipOne)
                {
                    damage1 = w.damage;
                    speed1 = w.speed;
                    if (w is Gun)
                    {
                        Gun g = w as Gun;
                        bullets1 = g.bullets;
                        range1 = g.range;
                        weaponType1 = 2;
                        type1 = g.type;
                        special1 = g.special;
                        rlspeed1 = g.rlspeed;
                    }
                    else
                    {
                        weaponType1 = 1;
                    }
                }
                if (w.name == menus.equipTwo)
                {
                    damage2 = w.damage;
                    speed2 = w.speed;
                    if (w is Gun)
                    {
                        Gun g = w as Gun;
                        bullets2 = g.bullets;
                        range2 = g.range;
                        weaponType2 = 2;
                        type2 = g.type;
                        special2 = g.special;
                        rlspeed2 = g.rlspeed;
                    }
                    else
                    {
                        weaponType2 = 1;
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class Item
{
    public string name;
    public int value, sellValue, weight;
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
    public int bullets;
    public float range, rlspeed;
    public string type, special;
}
