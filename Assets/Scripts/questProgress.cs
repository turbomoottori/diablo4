using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class questProgress : MonoBehaviour {

    public UnityEvent progress;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            progress.Invoke();
        }
    }
}
