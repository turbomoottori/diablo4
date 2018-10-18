using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class newCollectible : MonoBehaviour
{
    //WORK IN PROGRESS
    //TEST 
    //IT'S A TEST
    //DO U GET IT
    //IT'S A TEST SCRIPT

    public string collectibleName;
    public int value, weight, id;
    public bool stackable;
    public Type itemType;

    public int weaponDamage;
    public float weaponSpeed, gunRange;
    public GunType gunType;

    public float batteryEnergy;

    [TextArea]
    public string bookText;

    void Start()
    {
        foreach (Collectibles c in gameControl.control.collectibles)
            if (c.cName == collectibleName && c.posX == transform.position.x && c.posZ == transform.position.z)
                Destroy(gameObject);
    }

    void AddItem()
    {
        switch (itemType)
        {
            case Type.Item:
                items.ownedItems.Add(new Item2()
                {
                    name = collectibleName,
                    id = id,
                    baseValue = value,
                    weight = weight,
                    stackable = stackable
                });

                break;
            case Type.Weapon:
                items.ownedItems.Add(new Weapon2()
                {
                    name = collectibleName,
                    id = id,
                    baseValue = value,
                    weight = weight,
                    stackable = stackable,
                    damage = weaponDamage,
                    speed = weaponSpeed
                });

                break;
            case Type.Gun:
                items.ownedItems.Add(new Gun2()
                {
                    name = collectibleName,
                    id = id,
                    baseValue = value,
                    weight = weight,
                    stackable = stackable,
                    damage = weaponDamage,
                    speed = weaponSpeed,
                    range = gunRange,
                    type = gunType
                });

                break;
            case Type.Book:
                items.ownedItems.Add(new Book2()
                {
                    name = collectibleName,
                    id = id,
                    stackable = stackable,
                    text = bookText,
                    baseValue = value,
                    weight = weight
                });

                break;
            case Type.Battery:

                int newId = items.nextBatteryId;
                items.nextBatteryId += 1;

                items.ownedItems.Add(new Battery2()
                {
                    name = collectibleName + newId.ToString(),
                    id = newId,
                    energy = batteryEnergy,
                    isEmpty = false,
                    stackable = stackable,
                    baseValue = value,
                    weight = weight
                });
                break;
        }
    }

    public void PickUp()
    {
        gameObject.GetComponent<interactable>().HideE();
        gameControl.control.collectibles.Add(new Collectibles()
        {
            posX = transform.position.x,
            posZ = transform.position.z,
            cName = collectibleName
        });

        AddItem();
        menus.ShowCollected(collectibleName);

        if (itemType == Type.Battery)
            GameObject.Find("Globals").GetComponent<battery>().UpdateBatteryList();

        Destroy(this.gameObject);
    }

    public enum Type
    {
        Item,
        Weapon,
        Gun,
        Book,
        Battery
    }
}


