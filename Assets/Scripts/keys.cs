using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keys : MonoBehaviour {

    //WIP
    public static KeyCode key_interact, key_inventory, key_dash;

	// Use this for initialization
	void Start () {
		
	}

    public static void DefaultKeys()
    {
        key_dash = KeyCode.LeftShift;
        key_interact = KeyCode.E;
        key_inventory = KeyCode.Tab;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
