using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class spawns : MonoBehaviour {

    public static List<SpawnPoint> spawnPoints;
    public static SpawnPoint nextSpawn;
    public static string nextSpawnName;
    public static PlayerPos loadPos;

    void Start () {
        if (spawnPoints == null)
            spawnPoints = new List<SpawnPoint>();

        CheckSpawnPoints();

        nextSpawn = spawnPoints.FirstOrDefault(s => s.spawnName == nextSpawnName);
        if (nextSpawn != null)
        {
            print("spawns in predetermined spot");
            Vector3 spawnPos = new Vector3(nextSpawn.position.x, nextSpawn.position.y, nextSpawn.position.z);
            GameObject.Find("Player").transform.position = spawnPos;
            gameControl.control.playerPos = nextSpawn.position;
        }
        else if (loadPos != null)
        {
            Vector3 pos = new Vector3(loadPos.x, loadPos.y, loadPos.z);
            GameObject.Find("Player").transform.position=pos;
            print("spawns in saved spot at " + pos.ToString());
        }
        
	}

    //finds every spawn point and saves their position
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

    public static void LoadLevel(string levelName, string spawnName)
    {
        nextSpawnName = spawnName;
        SceneManager.LoadScene(levelName);
    }
}

public class SpawnPoint
{
    public string spawnName;
    public PlayerPos position;
}
