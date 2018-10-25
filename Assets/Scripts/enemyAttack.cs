using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAttack : MonoBehaviour {
    
    public int dmg;
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
            StartCoroutine(other.gameObject.GetComponent<playerMovement>().Hurt(dmg));
    }*/
}
