using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keys : MonoBehaviour {

    //WIP
    public static SavedKeys savedKeys;


	// Use this for initialization
	void Start () {
		
	}

    public static void DefaultKeys()
    {
        savedKeys = new SavedKeys() {
            interactKey = KeyCode.E,
            inventoryKey = KeyCode.Tab,
            dashKey = KeyCode.LeftShift,
            slowmoKey = KeyCode.Alpha1,
        };
    }
	
	public static bool AcceptNewKey(KeyCode newKey)
    {
        if (newKey == KeyCode.Escape || newKey == KeyCode.Space)
            return false;
        if (newKey == savedKeys.interactKey || newKey == savedKeys.inventoryKey || newKey == savedKeys.dashKey || newKey == savedKeys.slowmoKey)
            return false;

        return true;
    }
}

[System.Serializable]
public class SavedKeys
{
    public KeyCode interactKey, inventoryKey, dashKey, slowmoKey;
}
