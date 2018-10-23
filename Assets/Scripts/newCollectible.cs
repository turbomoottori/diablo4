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
    public Battery battery;
    string collectibleName = " ";

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
                collectibleName = battery.name;
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
                    name = battery.name + newId.ToString(),
                    id = newId,
                    energy = battery.energy,
                    isEmpty = battery.isEmpty,
                    stackable = battery.stackable,
                    baseValue = battery.baseValue,
                    weight = battery.weight
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
        //menus.ShowCollected(collectibleName);

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


