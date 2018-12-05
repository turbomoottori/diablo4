using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class minigame : MonoBehaviour {

    public string minigameName;
    bool gameOn = false;
    public Camera minigameCam;
    Camera maincam;
    public int level;
    public int[] currentPos, finalPos;
    public GameObject[] parts;
    public float minPos, maxPos;
    float[] positions;
    float timer = 0f;
    float maxTimer = 0.2f;

	void Start () {
        //checks if minigame is already completed
        Minigame temp = quests.minigames.FirstOrDefault(q => q.name == minigameName);
        if (temp != null)
            if (temp.completed)
                print("already completed");
        else
            quests.minigames.Add(new Minigame() { name = minigameName, completed = false });


        minigameCam.enabled = false;
        maincam = GameObject.Find("Main Camera").GetComponent<Camera>();
        Positions();
	}

    //checks starting positions
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
	
	void Update () {
        //for testing only
        if (!gameOn && Input.GetKeyDown(KeyCode.H))
            StartMiniGame();

        if (gameOn)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(keys.savedKeys.inventoryKey))
                ExitMiniGame();

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

            if (!PositionCheck())
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].transform.localPosition.y != positions[currentPos[i]])
                    {
                        Vector3 newPos = parts[i].transform.localPosition;
                        Vector3 targetPos = parts[i].transform.localPosition;
                        targetPos.y = positions[currentPos[i]];
                        parts[i].transform.localPosition = Vector3.Lerp(newPos, targetPos, 0.1f);
                    }
                }
            }

            //replace with something more exciting
            if (CorrectPositions())
            {
                quests.QuestCompleted(minigameName);
                ExitMiniGame();
            }
        }
	}

    void PressButton(int buttonNumber)
    {
        switch (buttonNumber)
        {
            case 1:
                if (CanMoveUp(0, 1) && CanMoveUp(3, 1))
                {
                    currentPos[0] += 1;
                    currentPos[3] += 1;
                }
                break;
            case 2:
                if (CanMoveDown(1, 1) && CanMoveDown(3, 1))
                {
                    currentPos[1] -= 1;
                    currentPos[3] -= 1;
                }
                break;
            case 3:
                if (CanMoveUp(1, 1) && CanMoveUp(2, 1))
                {
                    currentPos[1] += 1;
                    currentPos[2] += 1;
                }
                break;
            case 4:
                if (CanMoveDown(0, 1) && CanMoveDown(2,1))
                {
                    currentPos[0] -= 1;
                    currentPos[2] -= 1;
                }
                break;
        }
    }

    //checks if part can be moved up
    bool CanMoveUp(int part, int plus)
    {
        if (currentPos[part] + plus <= level - 1)
            return true;

        return false;
    }

    //checks if part can be moved down
    bool CanMoveDown(int part, int minus)
    {
        if (currentPos[part] - minus >= 0)
            return true;

        return false;
    }

    //checks if player has won 
    bool CorrectPositions()
    {
        for(int i = 0; i < finalPos.Length; i++)
            if (currentPos[i] != finalPos[i])
                return false;

        return true;
    }

    //checks if objects need to be moved
    bool PositionCheck()
    {
        for (int i = 0; i < parts.Length; i++)
            if (parts[i].transform.localPosition.y != positions[currentPos[i]])
                return false;

        return true;
    }

    public void StartMiniGame()
    {
        //do transition animation or something
        ui.minigame = true;
        maincam.enabled = false;
        minigameCam.enabled = true;
        gameOn = true;
        ui.TogglePause();
    }

    public void ExitMiniGame()
    {
        //some animation here too
        ui.minigame = false;
        maincam.enabled = true;
        minigameCam.enabled = false;
        gameOn = false;
        ui.TogglePause();
    }
}
