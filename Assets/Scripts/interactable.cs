using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactable : MonoBehaviour {

    GameObject e;
    public Type type;
    [Tooltip("use only if type is door")]
    public string levelToLoad;
    [Tooltip("the name of the gameobject this door will take to")]
    public string nextPositionName;
    [Tooltip("use only if this object is delivery location")]
    public string deliveryQuest;

    private void Start()
    {
        e = Instantiate(Resources.Load("ui/interact") as GameObject, GameObject.Find("Canvas").transform, false);
        e.SetActive(false);
    }

    private void Update()
    {
        e.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        if (ui.anyOpen)
            HideE();
    }

    public void ShowE()
    {
        if(!ui.anyOpen)
            e.SetActive(true);
        if (type == Type.merchant)
            GetComponent<merchant>().ChangeItems();
    }

    public void HideE()
    {
        e.SetActive(false);
    }

    public enum Type
    {
        collectible,
        merchant,
        chest,
        bookcase,
        npc,
        door,
        deliveryLocation,
        workbench
    }
}
