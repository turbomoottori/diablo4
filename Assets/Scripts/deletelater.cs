using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class deletelater : MonoBehaviour {

    //THIS SCRIPT IS JUST FOR DEMO VERSION
    //DELETE LATER

    Scene current;

	// Use this for initialization
	void Start () {
        current = SceneManager.GetActiveScene();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown && current.name=="endscene")
            SceneManager.LoadScene("mainmenu");
	}

    public IEnumerator EndGame(string scenename)
    {
        Image f = Instantiate(Resources.Load("ui/fade") as GameObject, GameObject.Find("Canvas").transform, false).GetComponent<Image>();
        Color fadeColor = f.color;
        float fade = 0;
        while (fade <= 1)
        {
            fadeColor.a = fade;
            f.color = fadeColor;
            fade += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(scenename);
    }
}
