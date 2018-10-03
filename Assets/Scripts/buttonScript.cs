using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class buttonScript : MonoBehaviour, IPointerClickHandler {

    GameObject m;
<<<<<<< HEAD
    public buttonType type;
=======
    public int itemType;
>>>>>>> 51ccfac11b0168bb9bb43fd8f3c61ae93e077624

    private void Start()
    {
        m = GameObject.Find("Globals");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
<<<<<<< HEAD
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
            case buttonType.sell:
                if (eventData.button == PointerEventData.InputButton.Left)
                    m.GetComponent<menus>().MerchClick(gameObject.name, false);
                break;
            case buttonType.buy:
                if (eventData.button == PointerEventData.InputButton.Left)
                    m.GetComponent<menus>().MerchClick(gameObject.name, true);
                break;
            case buttonType.expensive:
                if (eventData.button == PointerEventData.InputButton.Left)
                    m.GetComponent<menus>().TooExpensive();
                break;
=======
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
>>>>>>> 51ccfac11b0168bb9bb43fd8f3c61ae93e077624
        }
        
    }

    public enum buttonType
    {
        equip,
        storable,
        stored,
        sell,
        buy,
        expensive
    }
}
