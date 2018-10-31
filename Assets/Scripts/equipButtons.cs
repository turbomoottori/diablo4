using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class equipButtons : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int equipNum;
    GameObject g;

    void Start()
    {
        g = GameObject.Find("Globals");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        g.GetComponent<ui>().ClickEquip(equipNum);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        g.GetComponent<ui>().HoverEquip(equipNum);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        g.GetComponent<ui>().StopHover();
    }
}
