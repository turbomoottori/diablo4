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
    public Type itemType;
    public Item item;
    public Weapon weapon;
    public Gun gun;
    public Book book;
    public float batteryEnergy;
    //public Battery battery;
    string collectibleName = " ";
    public int ammoAmount;

    void Start()
    {
        switch (itemType)
        {
            case Type.Item:
                collectibleName = item.name;
                break;
            case Type.Weapon:
                collectibleName = weapon.name;
                break;
            case Type.Gun:
                collectibleName = gun.name;
                break;
            case Type.Book:
                collectibleName = book.name;
                break;
            case Type.Battery:
                collectibleName = "Battery";
                break;
            case Type.BasicAmmo:
                collectibleName = "+ " +ammoAmount.ToString() + " basic ammo";
                break;
            case Type.RapidAmmo:
                collectibleName = "+ " + ammoAmount.ToString() + " rapid ammo";
                break;
            case Type.ShotgunAmmo:
                collectibleName = "+ " + ammoAmount.ToString() + " shotgun ammo";
                break;
        }

        foreach (Collectibles c in gameControl.control.collectibles)
            if (c.cName == collectibleName && c.posX == transform.position.x && c.posZ == transform.position.z)
                Destroy(gameObject);
    }

    void AddItem()
    {
        switch (itemType)
        {
            case Type.Item:
                items.ownedItems.Add(item);
                break;
            case Type.Weapon:
                items.ownedItems.Add(weapon);
                break;
            case Type.Gun:
                items.ownedItems.Add(gun);
                break;
            case Type.Book:
                items.ownedItems.Add(book);
                break;
            case Type.Battery:

                int newId = items.nextBatteryId;
                items.nextBatteryId += 1;

                items.ownedItems.Add(new Battery()
                {
                    name = collectibleName + newId.ToString(),
                    id = newId,
                    energy = batteryEnergy,
                    isEmpty = false,
                    stackable = false,
                    baseValue = 100,
                    weight = 1
                });
                break;
            case Type.BasicAmmo:
                gameControl.basicAmmo += ammoAmount;
                break;
            case Type.RapidAmmo:
                gameControl.rapidAmmo += ammoAmount;
                break;
            case Type.ShotgunAmmo:
                gameControl.shotgunAmmo += ammoAmount;
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
        GameObject.Find("Globals").GetComponent<ui>().ShowCollectedItem(collectibleName);


        if (itemType == Type.Battery)
            items.CheckBatteries();

        Destroy(this.gameObject);
    }

    public enum Type
    {
        Item,
        Weapon,
        Gun,
        Book,
        Battery,
        BasicAmmo,
        RapidAmmo,
        ShotgunAmmo
    }
}


