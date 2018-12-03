using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class gameControl : MonoBehaviour
{
    public static gameControl control;

    public int hp, maxhp, money;
    public bool knowsDoubleJump, knowsDash, knowsSlowTime, autoBattery;

    public int volume;

    public static string equipOne, equipTwo;
    public static int batteryId;

    public static int basicAmmo, shotgunAmmo, rapidAmmo;

    public static List<Item> invItems = new List<Item>();
    public static List<Item> itemsStored = new List<Item>();
    public static List<Book> bookcaseBooks = new List<Book>();

    public List<NPC> npcs;
    public List<MerchantData> merchs;
    public List<Collectibles> collectibles;
    public List<EnemyList> enemies;
    public List<QuestGivers> questGivers;
    public string currentScene;
    public PlayerPos playerPos;

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
        if (questGivers == null)
            questGivers = new List<QuestGivers>();

        //delete later
        basicAmmo = 5;
        rapidAmmo = 20;
        shotgunAmmo = 50;
        money = 100;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += LevelLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= LevelLoaded;
    }

    void LevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "mainmenu")
            AutoSave();
        currentScene = scene.name;
        print(currentScene);
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
        data.equip1 = equipOne;
        data.equip2 = equipTwo;
        data.invItems = invItems;
        data.itemsStored = itemsStored;
        data.knowsSlowTime = knowsSlowTime;
        data.questList = quests.questList;
        data.merchs = merchs;
        data.collectibles = collectibles;
        data.enemies = enemies;
        data.bookcaseBooks = bookcaseBooks;
        data.basicAmmo = basicAmmo;
        data.shotgunAmmo = shotgunAmmo;
        data.rapidAmmo = rapidAmmo;
        data.autoBattery = autoBattery;
        data.batteryId = batteryId;
        data.playerPos = new PlayerPos() { x = playerMovement.savedPos.x, y = playerMovement.savedPos.y, z = playerMovement.savedPos.z };
        data.minigames = quests.minigames;
        data.questGivers = questGivers;
        data.npcs = npcs;

        Scene cScene = SceneManager.GetActiveScene();
        data.currentScene = cScene.name;

        //serializes and closes file
        bf.Serialize(file, data);
        file.Close();
    }

    public void AutoSave()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        //creates new save file
        file = File.Create(Application.persistentDataPath + "/autosave.dat");
        PlayerData data = new PlayerData();

        //stores all relevant data
        data.hp = hp;
        data.maxhp = maxhp;
        data.money = money;
        data.knowsDoubleJump = knowsDoubleJump;
        data.knowsDash = knowsDash;
        data.equip1 = equipOne;
        data.equip2 = equipTwo;
        data.invItems = invItems;
        data.itemsStored = itemsStored;
        data.knowsSlowTime = knowsSlowTime;
        data.questList = quests.questList;
        data.merchs = merchs;
        data.collectibles = collectibles;
        data.enemies = enemies;
        data.bookcaseBooks = bookcaseBooks;
        data.basicAmmo = basicAmmo;
        data.shotgunAmmo = shotgunAmmo;
        data.rapidAmmo = rapidAmmo;
        data.autoBattery = autoBattery;
        data.batteryId = batteryId;
        if(spawns.nextSpawn!=null)
            data.playerPos = spawns.nextSpawn.position;
        data.minigames = quests.minigames;
        data.questGivers = questGivers;
        data.npcs = npcs;

        Scene cScene = SceneManager.GetActiveScene();
        data.currentScene = cScene.name;

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
            equipOne = data.equip1;
            equipTwo = data.equip2;
            invItems = data.invItems;
            itemsStored = data.itemsStored;
            knowsSlowTime = data.knowsSlowTime;
            quests.questList = data.questList;
            merchs = data.merchs;
            collectibles = data.collectibles;
            enemies = data.enemies;
            bookcaseBooks = data.bookcaseBooks;
            basicAmmo = data.basicAmmo;
            shotgunAmmo = data.shotgunAmmo;
            rapidAmmo = data.rapidAmmo;
            autoBattery = data.autoBattery;
            batteryId = data.batteryId;
            currentScene = data.currentScene;
            playerPos = data.playerPos;
            spawns.loadPos = playerPos;
            quests.minigames = data.minigames;
            questGivers = data.questGivers;
            npcs = data.npcs;

            SceneManager.LoadScene(data.currentScene);
        }
    }

    public void AutoLoad()
    {
        //check if file exists
        if (File.Exists(Application.persistentDataPath + "/autosave.dat"))
        {
            //loads file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/autosave.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            //sets all fetched data
            hp = data.hp;
            maxhp = data.maxhp;
            money = data.money;
            knowsDoubleJump = data.knowsDoubleJump;
            knowsDash = data.knowsDash;
            equipOne = data.equip1;
            equipTwo = data.equip2;
            invItems = data.invItems;
            itemsStored = data.itemsStored;
            knowsSlowTime = data.knowsSlowTime;
            quests.questList = data.questList;
            merchs = data.merchs;
            collectibles = data.collectibles;
            enemies = data.enemies;
            bookcaseBooks = data.bookcaseBooks;
            basicAmmo = data.basicAmmo;
            shotgunAmmo = data.shotgunAmmo;
            rapidAmmo = data.rapidAmmo;
            autoBattery = data.autoBattery;
            batteryId = data.batteryId;
            currentScene = data.currentScene;
            playerPos = data.playerPos;
            quests.minigames = data.minigames;
            questGivers = data.questGivers;
            npcs = data.npcs;

            SceneManager.LoadScene(data.currentScene);
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
        data.savedKeys = keys.savedKeys;

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
            keys.savedKeys = data.savedKeys;
        }
    }
}

[System.Serializable]
class PlayerData
{
    public int hp, maxhp, money, basicAmmo, shotgunAmmo, rapidAmmo, batteryId;
    public bool knowsDoubleJump, knowsDash, knowsSlowTime, autoBattery;
    public string equip1, equip2;
    public List<Item> invItems, itemsStored;
    public List<Quest> questList;
    public List<MerchantData> merchs;
    public List<Collectibles> collectibles;
    public List<EnemyList> enemies;
    public List<QuestGivers> questGivers;
    public List<Book> bookcaseBooks;
    public string currentScene;
    public PlayerPos playerPos;
    public List<Minigame> minigames;
    public List<NPC> npcs;
}


[System.Serializable]
public class MerchantData
{
    public string merchantName;
    public int basicAmmo, rapidAmmo, shotgunAmmo;
    public List<Item> merchantItems;
}

[System.Serializable]
class OptionsData
{
    public int volume;
    public SavedKeys savedKeys;
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

[System.Serializable]
public class NPC
{
    public string name;
    public bool canBeEnemy, enemyKilled, hasQuest, questCompleted;
    public int dialoqueState;
}

[System.Serializable]
public class QuestGivers
{
    public string name;
    public int questStage;
    public bool questCompleted;
}

[System.Serializable]
public class PlayerPos
{
    public float x, y, z;
}

[System.Serializable]
public class Minigame
{
    public string name;
    public bool completed;
}