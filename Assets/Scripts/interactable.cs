using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactable : MonoBehaviour {

    GameObject e;
    public Type type;

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
        e.SetActive(true);
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
    }
}
