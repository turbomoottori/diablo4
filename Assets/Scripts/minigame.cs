using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minigame : MonoBehaviour {

    bool gameOn = false;
    public Camera minigameCam;
    Camera maincam;
    public int level;
    public int[] currentPos, finalPos;
    public GameObject[] parts;
    public float minPos, maxPos;
    float[] positions;

	// Use this for initialization
	void Start () {
        minigameCam.enabled = false;
        maincam = GameObject.Find("Main Camera").GetComponent<Camera>();
        Positions();
	}

    void Positions()
    {
        positions = new float[level];
        float temp = (maxPos - minPos) / (positions.Length - 1);
        positions[0] = minPos;
        positions[positions.Length - 1] = maxPos;

        for (int j = 0; j < positions.Length; j++)
        {
            if (j != 0 && j != positions.Length - 1)
                positions[j] = minPos + (temp * j);
        }

        for (int i = 0; i < parts.Length; i++)
        {
            Vector3 newPos = parts[i].transform.localPosition;
            newPos.y = positions[currentPos[i]];
            parts[i].transform.localPosition = newPos;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.H))
            StartMiniGame();

        if (gameOn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                var ray = minigameCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "minigameButton")
                        PressButton(int.Parse(hit.collider.name));
                }
            }
        }
	}

    void PressButton(int buttonNumber)
    {
        switch (buttonNumber)
        {
            case 1:
                print("button 1 pressed");
                break;
            case 2:
                print("button 2 pressed");
                break;
            case 3:
                print("button 3 pressed");
                break;
            case 4:
                print("button 4 pressed");
                break;
        }
    }

    bool CorrectPositions()
    {
        for(int i = 0; i < finalPos.Length; i++)
            if (currentPos[i] != finalPos[i])
                return false;

        return true;
    }

    public void StartMiniGame()
    {
        //do transition animation or something
        maincam.enabled = false;
        minigameCam.enabled = true;
        gameOn = true;
        //then load the assets
        //make buttons
        //game starts
    }
}
