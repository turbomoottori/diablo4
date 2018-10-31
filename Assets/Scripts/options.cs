using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class options : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (transform.parent.name == "Volume")
            transform.parent.Find("Value").GetComponent<Text>().text = gameControl.control.volume.ToString();
	}
	
	public void Volume()
    {
        float newVolume = GetComponent<Slider>().value / 1 * 100;
        gameControl.control.volume = (int)newVolume;
        transform.parent.Find("Value").GetComponent<Text>().text = gameControl.control.volume.ToString();
    }
}
