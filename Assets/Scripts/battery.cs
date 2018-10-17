using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class battery : MonoBehaviour {

    float batteryDrain = 0.2f;
    public List<Battery> batteries;
    public List<Battery> allBatteries;
    Battery batteryInUse;
    public static bool batteryOn;

    // Use this for initialization
    void Start () {
        UpdateBatteryList();
	}

    //NOTE TO SELF
    //BATTERIES LOSE ENERGY WHEN IN STORAGE
    //FIX PLS
	
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
                    print(batteryInUse.energy);
                    batteryInUse.name = "Battery " + (batteryInUse.energy / 1 * 100).ToString("F0") + "%";
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
                    batteryInUse.name = "Battery " + (batteryInUse.energy / 1 * 100).ToString("F0") + "%";
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

    public void UpdateBatteryList()
    {
        foreach (Battery b in gameControl.invItems)
        {
            if(!allBatteries.Exists(Battery => Battery.id == b.id))
                allBatteries.Add(b);

            if (!batteries.Exists(Battery => Battery.id == b.id) && !b.isEmpty)
                batteries.Add(b);
        }

        batteries.RemoveAll(b => b.isEmpty == true);
        print(allBatteries.Count + " full batteries: " + batteries.Count);

        print("battery list updated");
    }
}
