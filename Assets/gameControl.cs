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
            Weapon firstSword = new Weapon() { name = "First Sword", damage = 3, speed = 5, value = 0, weight = 1 }; 
            Item test = new Item() { name = "test", value = 0, weight = 1 }; //delete later
            menus.invItems.Add(firstSword);
            menus.invItems.Add(test);
            menus.equipOne = "First Sword";
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
}

[System.Serializable]
class OptionsData
{
    public int volume;
}
