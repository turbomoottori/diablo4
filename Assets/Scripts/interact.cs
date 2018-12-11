using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interact : MonoBehaviour {

    GameObject lastClosest = null;
	
	// Update is called once per frame
	void Update () {
        bool interacting = CanInteract();

        if (interacting)
            CheckObjects();
        else
            ui.interactableObject = null;

        if (ui.anyOpen)
            lastClosest = null;
    }

    public bool CanInteract()
    {
        if (ui.minigame == true)
            return false;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.8f);
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.8f);
        float minDist = Mathf.Infinity;
        GameObject closest = null;

        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].gameObject.GetComponent<interactable>() != null)
            {
                Vector3 colliderDirection = hitColliders[i].transform.position - transform.position;
                float angleToObject = Vector3.Angle(colliderDirection, transform.forward);

                if (angleToObject >= -70 && angleToObject <= 70)
                {
                    float dist = Vector3.Distance(hitColliders[i].transform.position, transform.position);
                    if (dist < minDist)
                    {
                        closest = hitColliders[i].gameObject;
                        minDist = dist;
                    }
                }
            }
            i++;
        }

        for (int j = 0; j < hitColliders.Length; j++)
        {
            if (hitColliders[j].gameObject == closest && lastClosest != closest)
                hitColliders[j].gameObject.GetComponent<interactable>().ShowE();
            else if (hitColliders[j].gameObject != closest && hitColliders[j].gameObject.GetComponent<interactable>() != null)
            {
                hitColliders[j].gameObject.GetComponent<interactable>().HideE();
            }
        }

        if (lastClosest != closest)
            lastClosest = closest;

        ui.interactableObject = closest;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<interactable>() != null)
            other.gameObject.GetComponent<interactable>().HideE();

        if (other.gameObject == lastClosest)
            lastClosest = null;
    }
}
