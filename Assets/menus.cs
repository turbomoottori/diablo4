using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menus : MonoBehaviour {

    GameObject player, optionsMenu, pauseMenu;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        optionsMenu = GameObject.Find("OptionsPause");
        pauseMenu = GameObject.Find("GeneralPause");
        optionsMenu.SetActive(false);
	}

    public void Continue()
    {
        player.GetComponent<playerMovement>().Pause();
    }

    public void Options()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void Back()
    {
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void SaveExit()
    {
        print("save and exit");
    }
}
