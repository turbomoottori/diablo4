using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class buttonScript : MonoBehaviour, IPointerClickHandler {

    GameObject m;
    public buttonType type;

    private void Start()
    {
        m = GameObject.Find("Globals");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (type)
        {
            case buttonType.equip:
                if (eventData.button == PointerEventData.InputButton.Left)
                    m.GetComponent<menus>().InventoryClick(true, gameObject.name);
                else if (eventData.button == PointerEventData.InputButton.Right)
                    m.GetComponent<menus>().InventoryClick(false, gameObject.name);
                break;
            case buttonType.storable:
                if (eventData.button == PointerEventData.InputButton.Left)
                    m.GetComponent<menus>().InventoryClickStorage(gameObject.name, false);
                break;
            case buttonType.stored:
                if (eventData.button == PointerEventData.InputButton.Left)
                    m.GetComponent<menus>().InventoryClickStorage(gameObject.name, true);
                break;
        }
        
    }

    public enum buttonType
    {
        equip,
        storable,
        stored
    }
}
