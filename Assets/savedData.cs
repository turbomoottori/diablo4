using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class savedData {

    public static bool knowsDoubleJump, knowsDash, hasGun;
    public static int hp, maxhp, money;
    public static string activeLevel;

    static void SaveData()
    {
        PlayerPrefs.SetInt("KnowsDoubleJump", (knowsDoubleJump ? 1 : 0));
        PlayerPrefs.SetInt("KnowsDash", (knowsDash ? 1 : 0));
        PlayerPrefs.SetInt("HasGun", (hasGun ? 1 : 0));
        PlayerPrefs.SetInt("HP", hp);
        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.SetString("ActiveLevel", activeLevel);
    }

    static void LoadData()
    {
        knowsDoubleJump = (PlayerPrefs.GetInt("KnowsDoubleJump") != 0);
        knowsDash = (PlayerPrefs.GetInt("KnowsDash") != 0);
        hasGun = (PlayerPrefs.GetInt("HasGun") != 0);
        hp = PlayerPrefs.GetInt("HP");
        money = PlayerPrefs.GetInt("Money");
        activeLevel = PlayerPrefs.GetString("ActiveLevel");
    }
}
