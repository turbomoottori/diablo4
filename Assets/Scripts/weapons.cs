using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapons : MonoBehaviour {

    public static weapons w;

    public static int damage1, damage2;
    public static float speed1, speed2;
    public static int bullets1, bullets2;
    public static float range1, range2;

    //0 is no weapon, 1 is sword, 2 is gun
    public static int weaponTypeOne, weaponTypeTwo;

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
        foreach(Weapon w in menus.invItems)
        {
            if (w.name == menus.equipOne)
            {
                damage1 = w.damage;
                speed1 = w.speed;
                if(w is Gun)
                {
                    Gun g = w as Gun;
                    bullets1 = g.bullets;
                    range1 = g.range;
                    weaponTypeOne = 2;
                } else
                {
                    weaponTypeOne = 1;
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
                    weaponTypeTwo = 2;
                } else
                {
                    weaponTypeTwo = 1;
                }
            }
        }
    }
}

public class Item
{
    public string name;
    public int value, weight;
}

public class Weapon : Item
{
    public int damage;
    public float speed;
}

public class Gun : Weapon
{
    public int bullets;
    public float range;
}
