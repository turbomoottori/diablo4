using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class buttonScript : MonoBehaviour {

    GameObject m;

    private void Start()
    {
        m = GameObject.Find("Globals");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            m.GetComponent<menus>().InventoryClick(true);
        } else if (eventData.button == PointerEventData.InputButton.Right)
        {
            m.GetComponent<menus>().InventoryClick(false);
        }
    }
}
