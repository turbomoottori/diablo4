﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class gameControl : MonoBehaviour
{
    public static gameControl control;

    public int hp, maxhp, money;
    public bool knowsDoubleJump, knowsDash, knowsSlowTime;

    public int volume;

    public List<MerchantData> merchs;
    public List<Collectibles> collectibles;
    public List<EnemyList> enemies;

    void Awake()
    {
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

        if (merchs == null)
            merchs = new List<MerchantData>();
        if (collectibles == null)
            collectibles = new List<Collectibles>();
        if (enemies == null)
            enemies = new List<EnemyList>();
    }

    public void SaveGame(int saveFile)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        //creates new save file
        file = File.Create(Application.persistentDataPath + "/save" + saveFile + ".dat");
        PlayerData data = new PlayerData();

        //stores all relevant data
        data.hp = hp;
        data.maxhp = maxhp;
        data.money = money;
        data.knowsDoubleJump = knowsDoubleJump;
        data.knowsDash = knowsDash;
        data.equip1 = menus.equipOne;
        data.equip2 = menus.equipTwo;
        data.invItems = menus.invItems;
        data.itemsStored = menus.itemsStored;
        data.knowsSlowTime = knowsSlowTime;
        data.questList = quests.questList;
        data.merchs = merchs;
        data.collectibles = collectibles;
        data.enemies = enemies;

        //serializes and closes file
        bf.Serialize(file, data);
        file.Close();
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
            menus.equipOne = data.equip1;
            menus.equipTwo = data.equip2;
            menus.invItems = data.invItems;
            menus.itemsStored = data.itemsStored;
            knowsSlowTime = data.knowsSlowTime;
            quests.questList = data.questList;
            merchs = data.merchs;
            collectibles = data.collectibles;
            enemies = data.enemies;
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
    public bool knowsDoubleJump, knowsDash, knowsSlowTime;
    public string equip1, equip2;
    public List<Item> invItems, itemsStored;
    public List<Quest> questList;
    public List<MerchantData> merchs;
    public List<Collectibles> collectibles;
    public List<EnemyList> enemies;
}

[System.Serializable]
public class MerchantData
{
    public string merchantName;
    public List<Item> merchantItems;
}

[System.Serializable]
class OptionsData
{
    public int volume;
}

[System.Serializable]
public class Collectibles
{
    public float posX, posZ;
    public string cName;
}

[System.Serializable]
public class EnemyList
{
    public float posX, posZ, posY;
}
