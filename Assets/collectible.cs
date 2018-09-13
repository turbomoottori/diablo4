using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectible : MonoBehaviour {

    public string collectibleName;
    public int value, weight;
    public Type itemType;
    public int damage, bullets;
    public float speed, reloadSpeed, range;
    public string type, special;
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddItem()
    {
        if (itemType == Type.Item)
        {
            menus.invItems.Add(new Item() {
                name = collectibleName,
                value = value,
                weight = weight });
        }
        else if (itemType == Type.Weapon)
        {
            menus.invItems.Add(new Weapon() {
                name = collectibleName,
                value = value,
                weight = weight,
                damage = damage,
                speed = speed });
        }
        else if (itemType == Type.Gun)
        {
            menus.invItems.Add(new Gun() {
                name = collectibleName,
                value = value,
                weight = weight,
                damage = damage,
                speed = speed,
                bullets = bullets,
                range = range,
                rlspeed = reloadSpeed,
                special = special,
                type = type });
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "player")
        {
            AddItem();
            menus.ShowCollected(collectibleName);
            Destroy(this.gameObject);
        }
    }

    public enum Type
    {
        Item,
        Weapon,
        Gun,
    }
}
