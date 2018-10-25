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
            ui.interactableObject = null;
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
