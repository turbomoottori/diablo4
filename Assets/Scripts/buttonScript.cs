using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class buttonScript : MonoBehaviour, IPointerClickHandler
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

    public enum buttonType
    {
        inventoryItem,
        readyToSell,
        readyToBuy,
        cantSell,
        cantBuy,
        readyToStore,
        readyToTake
    }
}
