using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            // täällä on ongelma kun tämä objekti missä tämä skripti on, tuhotaan ja vihollinen jää jumiin
            StartCoroutine(other.gameObject.GetComponent<enemy>().Attacked(transform.parent.position));
        }
    }
}
