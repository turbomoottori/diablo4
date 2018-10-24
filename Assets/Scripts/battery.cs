using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class battery : MonoBehaviour {
    /*
    //fix this too
    //maybe put it all to items.cs???????
    //or smth idk
    //batteries cause a bug where if one is active and one is put in a chest, the one put away will still show up in inventory
    //and cause an error ofc
    //so idk i guess u should go and fix it

    float batteryDrain = 0.2f;
    public List<Battery> batteries;
    public List<Battery> allBatteries;
    Battery batteryInUse;
    public static bool batteryOn;
    public static string batteryName;

    // Use this for initialization
    void Start () {
        UpdateBatteryList();
	}
	
	// Update is called once per frame
	void Update () {
        switch (gameControl.control.autoBattery)
        {
            case true:
                if (batteries.Count > 0 && !batteryOn)
                    ChangeBattery();

                if (batteryOn)
                {
                    batteryInUse.energy -= batteryDrain * Time.deltaTime;
                    batteryInUse.name = "Battery" + batteryInUse.id;
                    batteryName = batteryInUse.name;
                    if (batteryInUse.energy <= 0 && batteries.Count > 1)
                    {
                        ChangeBattery();
                    }
                    else if (batteryInUse.energy <= 0 && batteries.Count <= 1)
                    {
                        batteryOn = false;
                        UpdateBatteryList();
                    }
                }
                break;
            case false:
                if (batteryOn)
                {
                    batteryInUse.energy -= batteryDrain * Time.deltaTime;
                    batteryInUse.name = "Battery" + batteryInUse.id;
                    batteryName = batteryInUse.name;
                    if (batteryInUse.energy <= 0)
                    {
                        UpdateBatteryList();
                        batteryOn = false;
                    }
                }
                break;
        }
	}

    void ChangeBattery()
    {
        if (batteryInUse != null && batteryInUse.energy < 0)
        {
            batteryInUse.isEmpty = true;
            UpdateBatteryList();
        }

        batteryInUse = batteries.FirstOrDefault();
        if (batteryInUse != null)
        {
            gameControl.batteryId = batteryInUse.id;
            print("energy: " + batteryInUse.energy);
            batteryOn = true;
        }
        else
        {
            batteryOn = false;
        }
    }

    public void UseSpecificBattery(Battery b)
    {
        batteryInUse = batteries.FirstOrDefault(Battery => Battery == b);
        gameControl.batteryId = batteryInUse.id;
        batteryOn = true;
    }

    public void UpdateBatteryList()
    {
        foreach (Item i in gameControl.invItems)
        {
            if (i is Battery)
            {
                Battery b = i as Battery;
                if (!allBatteries.Exists(Battery => Battery.id == b.id))
                    allBatteries.Add(b);

                if (!batteries.Exists(Battery => Battery.id == b.id) && !b.isEmpty)
                    batteries.Add(b);
            }
        }

        batteries.RemoveAll(b => b.isEmpty == true);
        print("total batteries: " + allBatteries.Count + ", full batteries: " + batteries.Count);

        print("battery list updated");
    }*/
}
