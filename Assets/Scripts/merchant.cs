using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class merchant : MonoBehaviour {

    public float priceMultiplier;
    GameObject globals;
    public int basicAmmo, rapidAmmo, shotgunAmmo;
    public List<Item> mitems;
    public List<Weapon> swords;
    public List<Gun> guns;
    public List<Book> books;

    // Use this for initialization
    void Start () {
        MerchantData m = gameControl.control.merchs.FirstOrDefault(i => i.merchantName == gameObject.name);

        //load merchant's items
        if (m != null)
        {
            mitems = new List<Item>();
            mitems = m.merchantItems;
            basicAmmo = m.basicAmmo;
            rapidAmmo = m.rapidAmmo;
            shotgunAmmo = m.shotgunAmmo;
        }
        else if (m == null)
        {
            if (swords != null)
                foreach (Weapon w in swords)
                    mitems.Add(w);

            if (guns != null)
                foreach (Gun g in guns)
                    mitems.Add(g);

            if (books != null)
                foreach (Book b in books)
                    mitems.Add(b);

            if (items.ownedItems != null)
            {
                foreach (Item i in items.ownedItems)
                    CheckItemDuplicates(i.name);
            }

            gameControl.control.merchs.Add(new MerchantData()
            {
                merchantName = gameObject.name,
                basicAmmo = basicAmmo,
                rapidAmmo = rapidAmmo,
                shotgunAmmo = shotgunAmmo,
                merchantItems = mitems
            });
        }
    }

    public void SaveAmmoAmont()
    {
        MerchantData m = gameControl.control.merchs.FirstOrDefault(i => i.merchantName == gameObject.name);
        m.basicAmmo = basicAmmo;
        m.rapidAmmo = rapidAmmo;
        m.shotgunAmmo = shotgunAmmo;
    }

    //removes items player already owns
    void CheckItemDuplicates(string name)
    {
        Item temp = mitems.FirstOrDefault(i => i.name == name);
        if (temp != null && !temp.stackable)
            mitems.Remove(temp);
    }

    public void ChangeItems()
    {
        ui.merchantAmmoB = basicAmmo;
        ui.merchantAmmoR = rapidAmmo;
        ui.merchantAmmoS = shotgunAmmo;
        ui.merchantItems = mitems;
        ui.priceMultiplier = priceMultiplier;
    }
}
