using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropItem : MonoBehaviour {

    public newCollectible.Type itemType;
    public Item itemDrop;
    public Weapon weaponDrop;
    public Consumable consumableDrop;
    public Gun gunDrop;
    public Book bookDrop;
    public float batteryEnergy;
    public int bAmmo, rAmmo, sAmmo;
	
	public void SpawnItem()
    {
        GameObject c;
        c = Instantiate(Resources.Load("collectible") as GameObject, transform.position + Vector3.up * 2, transform.rotation);
        c.GetComponent<newCollectible>().itemType = itemType;
        switch (itemType)
        {
            case newCollectible.Type.Item:
                c.GetComponent<newCollectible>().item = itemDrop;
                break;
            case newCollectible.Type.Weapon:
                c.GetComponent<newCollectible>().weapon = weaponDrop;
                break;
            case newCollectible.Type.Gun:
                c.GetComponent<newCollectible>().gun = gunDrop;
                break;
            case newCollectible.Type.Book:
                c.GetComponent<newCollectible>().book = bookDrop;
                break;
            case newCollectible.Type.Battery:
                c.GetComponent<newCollectible>().batteryEnergy = batteryEnergy;
                break;
            case newCollectible.Type.Consumable:
                c.GetComponent<newCollectible>().consumable = consumableDrop;
                break;
            case newCollectible.Type.BasicAmmo:
                c.GetComponent<newCollectible>().ammoAmount = bAmmo;
                break;
            case newCollectible.Type.RapidAmmo:
                c.GetComponent<newCollectible>().ammoAmount = rAmmo;
                break;
            case newCollectible.Type.ShotgunAmmo:
                c.GetComponent<newCollectible>().ammoAmount = sAmmo;
                break;
        }
        c.GetComponent<Rigidbody>().AddForce(Vector3.up, ForceMode.Impulse);
    }
}
