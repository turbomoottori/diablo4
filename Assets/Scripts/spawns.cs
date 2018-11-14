using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class spawns : MonoBehaviour {

    public static List<SpawnPoint> spawnPoints;

    //WIP, MAKE SPAWN POSITION LIST FOR EACH LEVEL

	// Use this for initialization
	void Start () {
        if (spawnPoints == null)
            spawnPoints = new List<SpawnPoint>();

        CheckSpawnPoints();
	}

    void CheckSpawnPoints()
    {
        GameObject[] t = GameObject.FindGameObjectsWithTag("spawn");
        for (int i = 0; i < t.Length; i++)
        {
            if (spawnPoints.FirstOrDefault(s => s.spawnName == t[i].name) == null)
            {
                spawnPoints.Add(new SpawnPoint()
                {
                    spawnName = t[i].name,
                    position = new PlayerPos()
                    {
                        x = t[i].transform.position.x,
                        y = t[i].transform.position.y,
                        z = t[i].transform.position.z
                    }
                });
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class SpawnPoint
{
    public string spawnName;
    public PlayerPos position;
}
