using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interact : MonoBehaviour {

    InteractWith interactWith;
    GameObject target;

	// Use this for initialization
	void Start () {
		
	}
	
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
                menus.merchClose = true;
                menus.chestClose = false;
                menus.talkReady = false;
                menus.bcClose = false;
                target.GetComponent<merchant>().ChangeItems();
                break;
            case InteractWith.collectible:
                menus.merchClose = false;
                menus.chestClose = false;
                menus.talkReady = false;
                menus.bcClose = false;
                if (Input.GetKeyDown(KeyCode.E))
                    target.GetComponent<collectible>().PickUp();
                break;
            case InteractWith.chest:
                menus.chestClose = true;
                menus.merchClose = false;
                menus.talkReady = false;
                menus.bcClose = false;
                break;
            case InteractWith.bookcase:
                menus.bcClose = true;
                menus.merchClose = false;
                menus.chestClose = false;
                menus.talkReady = false;
                break;
            case InteractWith.npc:
                menus.merchClose = false;
                menus.chestClose = false;
                menus.talkReady = true;
                menus.bcClose = false;
                if (Input.GetKeyDown(KeyCode.E) && !menus.anyOpen)
                    target.GetComponent<npcSpeech>().Interact();
                break;
            case InteractWith.nobody:
                menus.merchClose = false;
                menus.chestClose = false;
                menus.talkReady = false;
                menus.bcClose = false;
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<interactable>() != null)
        {
            other.gameObject.GetComponent<interactable>().HideE();
        }
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
