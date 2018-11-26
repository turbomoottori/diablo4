using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keys : MonoBehaviour {

    //WIP
    public static SavedKeys savedKeys;
    public static KeyCode[] deniedKeys = new KeyCode[]
    {
        KeyCode.Escape,
        KeyCode.Space,
        KeyCode.W,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D,
    };

    public static void DefaultKeys()
    {
        savedKeys = new SavedKeys() {
            interactKey = KeyCode.E,
            inventoryKey = KeyCode.Tab,
            dashKey = KeyCode.LeftShift,
            slowtimeKey = KeyCode.Alpha1,
            attackKey = KeyCode.Mouse0,
            spAttackKey = KeyCode.Mouse1
        };
    }
	
	public static bool AcceptNewKey(KeyCode newKey)
    {
        KeyCode[] keysInUse = new KeyCode[]
        {
            savedKeys.interactKey,
            savedKeys.inventoryKey,
            savedKeys.dashKey,
            savedKeys.slowtimeKey,
            savedKeys.attackKey,
            savedKeys.spAttackKey
        };

        for(int i = 0; i < deniedKeys.Length; i++)
            if (newKey == deniedKeys[i])
                return false;


        for (int i = 0; i < keysInUse.Length; i++)
            if (newKey == keysInUse[i])
                return false;

        return true;
    }
}

[System.Serializable]
public class SavedKeys
{
    public KeyCode interactKey, inventoryKey, dashKey, slowtimeKey, attackKey, spAttackKey;
}

