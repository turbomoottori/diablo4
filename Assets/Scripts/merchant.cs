using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class merchant : MonoBehaviour {

    public float priceMultiplier;
    GameObject globals;
    public int basicAmmo, rapidAmmo, shotgunAmmo;
    public List<Item> items;
    public List<Weapon> swords;
    public List<Gun> guns;
    public List<Book> books;

    // Use this for initialization
    void Start () {
        MerchantData m = gameControl.control.merchs.FirstOrDefault(i => i.merchantName == gameObject.name);

        //load merchant's items
        if (m != null)
        {
            items = new List<Item>();
            items = m.merchantItems;
        }
        else if (m == null)
        {
            foreach (Weapon w in swords)
                items.Add(w);

            foreach (Gun g in guns)
                items.Add(g);

            foreach (Book b in books)
                items.Add(b);

            foreach (Item i in gameControl.invItems)
                CheckItemDuplicates(i.name);

            gameControl.control.merchs.Add(new MerchantData()
            {
                merchantName = gameObject.name,
                merchantItems = items
            });
        }
    }

    //removes items player already owns
    void CheckItemDuplicates(string name)
    {
        Item temp = items.FirstOrDefault(i => i.name == name);
        if (temp != null && !temp.stackable)
            items.Remove(temp);
    }

    public void ChangeItems()
    {
        ui.merchantAmmoB = basicAmmo;
        ui.merchantAmmoR = rapidAmmo;
        ui.merchantAmmoS = shotgunAmmo;
        ui.merchantItems = items;
        ui.priceMultiplier = priceMultiplier;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
