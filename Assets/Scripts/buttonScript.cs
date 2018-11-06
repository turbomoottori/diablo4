using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class buttonScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    GameObject g;
    public buttonType type;

    private void Start()
    {
        g = GameObject.Find("Globals");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            g.GetComponent<ui>().ClickedItem(true, type, gameObject.name);
        else
            g.GetComponent<ui>().ClickedItem(false, type, gameObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (type)
        {
            case buttonType.inventoryItem:
                g.GetComponent<ui>().HoverOnItem(gameObject.name, 1);
                break;
            case buttonType.readyToSell:
                g.GetComponent<ui>().HoverOnItem(gameObject.name, 1);
                break;
            case buttonType.readyToBuy:
                g.GetComponent<ui>().HoverOnItem(gameObject.name, 3);
                break;
            case buttonType.cantSell:
                g.GetComponent<ui>().HoverOnItem(gameObject.name, 1);
                break;
            case buttonType.cantBuy:
                g.GetComponent<ui>().HoverOnItem(gameObject.name, 3);
                break;
            case buttonType.readyToStore:
                g.GetComponent<ui>().HoverOnItem(gameObject.name, 1);
                break;
            case buttonType.readyToTake:
                g.GetComponent<ui>().HoverOnItem(gameObject.name, 2);
                break;
            case buttonType.ammo:
                g.GetComponent<ui>().HoverOnItem(gameObject.name, 4);
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        g.GetComponent<ui>().StopHover();
    }

    public enum buttonType
    {
        inventoryItem,
        readyToSell,
        readyToBuy,
        cantSell,
        cantBuy,
        readyToStore,
        readyToTake,
        ammo
    }
}
