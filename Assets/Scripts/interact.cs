using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interact : MonoBehaviour {

    InteractWith interactWith;
    GameObject target;
	
	// Update is called once per frame
	void Update () {
        bool interacting = CanInteract();

        if (interacting)
            CheckObjects();
        else
            interactWith = InteractWith.nobody;

        switch (interactWith)
        {
            case InteractWith.merchant:
                ui.closestInteractable = "merchant";
                target.GetComponent<merchant>().ChangeItems();
                break;
            case InteractWith.collectible:
                ui.closestInteractable = "collectible";
                break;
            case InteractWith.chest:
                ui.closestInteractable = "chest";
                break;
            case InteractWith.bookcase:
                ui.closestInteractable = "bookcase";
                break;
            case InteractWith.npc:
                ui.closestInteractable = "npc";
                break;
            case InteractWith.nobody:
                ui.closestInteractable = "nobody";
                break;
        }
    }

    public bool CanInteract()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.12f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].gameObject.GetComponent<interactable>() != null)
                return true;
            i++;
        }
        return false;
    }

    void CheckObjects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.12f);
        float minDist = Mathf.Infinity;
        GameObject closest = null;

        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].gameObject.GetComponent<interactable>() != null)
            {
                float dist = Vector3.Distance(hitColliders[i].transform.position, transform.position);
                if (dist < minDist)
                {
                    closest = hitColliders[i].gameObject;
                    minDist = dist;
                }
            }
            i++;
        }

        for (int j = 0; j < hitColliders.Length; j++)
        {
            if (hitColliders[j].gameObject == closest)
                hitColliders[j].gameObject.GetComponent<interactable>().ShowE();
            else if (hitColliders[j].gameObject != closest && hitColliders[j].gameObject.GetComponent<interactable>() != null)
            {
                hitColliders[j].gameObject.GetComponent<interactable>().HideE();
            }
        }

        switch (closest.gameObject.GetComponent<interactable>().type)
        {
            case interactable.Type.merchant:
                interactWith = InteractWith.merchant;
                break;
            case interactable.Type.collectible:
                interactWith = InteractWith.collectible;
                break;
            case interactable.Type.chest:
                interactWith = InteractWith.chest;
                break;
            case interactable.Type.bookcase:
                interactWith = InteractWith.bookcase;
                break;
            case interactable.Type.npc:
                interactWith = InteractWith.npc;
                break;
        }

        target = closest;
        ui.interactableObject = closest;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<interactable>() != null)
            other.gameObject.GetComponent<interactable>().HideE();
    }

    enum InteractWith
    {
        merchant,
        collectible,
        chest,
        bookcase,
        npc,
        nobody
    }
}
