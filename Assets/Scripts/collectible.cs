using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class collectible : MonoBehaviour {

    public string collectibleName;
    public int value, sellValue, weight;
    public bool stackable, canSell;
    public Type itemType;
    public int damage, bullets;
    public float speed, reloadSpeed, range;
    public string type, special;
    GameObject e;
    bool isClose = false;
	
	void Start () {
        e = Instantiate(Resources.Load("ui/interact", typeof(GameObject))) as GameObject;
        e.transform.SetParent(GameObject.Find("Canvas").transform, false);
        e.SetActive(false);

        foreach(Collectibles c in gameControl.control.collectibles)
            if (c.cName == collectibleName && c.posX == transform.position.x && c.posZ == transform.position.z)
                Destroy(gameObject);
    }

    void Update()
    {
        e.transform.position = Camera.main.WorldToScreenPoint(transform.position);

        if (isClose && Input.GetKeyDown(KeyCode.E))
            PickUp();
    }

    void AddItem()
    {
        if (itemType == Type.Item)
        {
            menus.invItems.Add(new Item() {
                name = collectibleName,
                value = value,
                sellValue = sellValue,
                weight = weight,
                canSell = canSell,
                stackable = stackable });
        }
        else if (itemType == Type.Weapon)
        {
            menus.invItems.Add(new Weapon() {
                name = collectibleName,
                canSell = canSell,
                value = value,
                sellValue = sellValue,
                weight = weight,
                damage = damage,
                speed = speed });
        }
        else if (itemType == Type.Gun)
        {
            menus.invItems.Add(new Gun() {
                name = collectibleName,
                canSell = canSell,
                value = value,
                sellValue = sellValue,
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

    void PickUp()
    {
        e.SetActive(false);

        gameControl.control.collectibles.Add(new Collectibles()
        {
            posX = transform.position.x,
            posZ=transform.position.z,
            cName = collectibleName
        });

        AddItem();
        menus.ShowCollected(collectibleName);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "interactzone")
        {
            e.SetActive(true);
            isClose = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "interactzone")
        {
            e.SetActive(false);
            isClose = false;
        }
    }

    public enum Type
    {
        Item,
        Weapon,
        Gun,
    }

    float Distance(Vector3 st, Vector3 en)
    {
        float d = 0.0f;

        float dx = st.x - en.x;
        float dy = st.y - en.y;
        float dz = st.z - en.z;

        d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);


        return d;
    }
}


