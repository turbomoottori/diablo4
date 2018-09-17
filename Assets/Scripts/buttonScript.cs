using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class buttonScript : MonoBehaviour, IPointerClickHandler {

    GameObject m;
    public int itemType;

    private void Start()
    {
        m = GameObject.Find("Globals");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemType == 1)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                m.GetComponent<menus>().InventoryClick(true, gameObject.name);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                m.GetComponent<menus>().InventoryClick(false, gameObject.name);
            }
        }
        else if (itemType == 2)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                m.GetComponent<menus>().TakeOrDeposit(gameObject.name, false);
            }
        }
        else if (itemType == 3)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                m.GetComponent<menus>().TakeOrDeposit(gameObject.name, true);
            }
        }
    }
}
