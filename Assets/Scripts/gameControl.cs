using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class gameControl : MonoBehaviour
{
    public static gameControl control;

    public int hp, maxhp, money;
    public bool knowsDoubleJump, knowsDash, hasSword;

    public int volume;

    void Awake()
    {
        hasSword = false;
        //prevents doubles
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //loads options automatically
        LoadOptions();

        //adds sword to inventory
        FirstSword();
    }

    //creates first sword
    void FirstSword()
    {
        if (!hasSword)
        {
            hasSword = true;
            Weapon firstSword = new Weapon() { name = "First Sword", damage = 3, speed = 0.2f, value = 0, weight = 1 }; 
            Item test = new Item() { name = "test", value = 0, weight = 1 }; //delete later
            Gun testGun = new Gun() { bullets = 5, damage = 2, name = "GUN", range = 20f, speed = 10f, value = 0, weight = 1, type="rapid", rlspeed=0.5f, special="big"};
            menus.invItems.Add(firstSword);
            menus.invItems.Add(test);
            menus.invItems.Add(testGun);

            menus.equipOne = "First Sword";
            weapons.damage1 = firstSword.damage;
            weapons.speed1 = firstSword.speed;
            weapons.weaponType1 = 1;

            menus.equipTwo = "Empty";
        }
    }

    public void SaveGame(int saveFile)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        //checks if save file already exists
        if (File.Exists(Application.persistentDataPath + "/save" + saveFile + ".dat"))
        {
            //overwrite prompt
            print("overwrite prompt");
        }
        else
        {
            //creates new save file
            file = File.Create(Application.persistentDataPath + "/save" + saveFile + ".dat");
            PlayerData data = new PlayerData();

            //stores all relevant data
            data.hp = hp;
            data.maxhp = maxhp;
            data.money = money;
            data.knowsDoubleJump = knowsDoubleJump;
            data.knowsDash = knowsDash;
            data.hasSword = hasSword;
            data.equip1 = menus.equipOne;
            data.equip2 = menus.equipTwo;
            data.invItems = menus.invItems;
            data.items = menus.items;
            data.storedItems = menus.storedItems;

            //serializes and closes file
            bf.Serialize(file, data);
            file.Close();
        }
    }

    public void LoadGame(int saveFile)
    {
        //check if file exists
        if (File.Exists(Application.persistentDataPath + "/save" + saveFile + ".dat"))
        {
            //loads file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save" + saveFile + ".dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            //sets all fetched data
            hp = data.hp;
            maxhp = data.maxhp;
            money = data.money;
            knowsDoubleJump = data.knowsDoubleJump;
            knowsDash = data.knowsDash;
            hasSword = data.hasSword;
            menus.equipOne = data.equip1;
            menus.equipTwo = data.equip2;
            menus.invItems = data.invItems;
            menus.items = data.items;
            menus.storedItems = data.storedItems;
        }
    }

    //saves options
    public void SaveOptions()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        file = File.Create(Application.persistentDataPath + "/options.dat");
        OptionsData data = new OptionsData();
        data.volume = volume;

        bf.Serialize(file, data);
        file.Close();
    }

    //loads options
    public void LoadOptions()
    {
        if (File.Exists(Application.persistentDataPath + "/options.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/options.dat", FileMode.Open);
            OptionsData data = (OptionsData)bf.Deserialize(file);
            file.Close();

            volume = data.volume;
        }
    }
}

[System.Serializable]
class PlayerData
{
    public int hp, maxhp, money;
    public bool knowsDoubleJump, knowsDash, hasSword;
    public string equip1, equip2;
    public List<Item> invItems;
    public List<GameObject> items, storedItems;
}

[System.Serializable]
class OptionsData
{
    public int volume;
}
