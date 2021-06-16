using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHordeManager : MonoBehaviour
{
    List<GameObject> enemyList = new List<GameObject>();
    public GameObject enemyPrefab;
    public GameObject gameManager;
    public int enemyCount = 0;
    public int numRows, enemyPerRow;
    public float[] enemySeperation = new float[2];
    public float downStep;
    public bool hitBorder = false;
    public bool gameRunning = false;
    public float startSpeed;
    float speed;
    float _x, _y;

    // Start is called before the first frame update
    void Start()
    {
        _x = transform.position.x;
        _y = transform.position.y;
        speed = startSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            Movement(transform.position.x, transform.position.y);
        }
    }

    public void SpawnEnemies()
    {
        transform.position = new Vector3(_x, _y, transform.position.z); //reset to starting position
        float xSep = enemySeperation[0];
        float ySep = enemySeperation[1];
        enemyList.Clear();

        float x = -(((float)enemyPerRow - 1f) * enemySeperation[0] / 2f) + transform.position.x; //Centralise the horde
        float y = transform.position.y;
        float z = transform.position.z;

        if (enemyCount > 0)
        {
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Destroy(enemy);
            }
            enemyCount = 0;
        }

        Debug.Log("ENEMY SPAWNING STARTED");
        for (int i = 0; i < numRows; i++)
        {
            for (int k = 0; k < enemyPerRow; k++)
            {
                enemyList.Add(enemyPrefab);
                enemyList[k] = Instantiate(enemyPrefab, new Vector3(x + (k * xSep), y - (i * ySep), z), transform.rotation);
                enemyList[k].transform.parent = transform;
                enemyCount++;
            }
        }

        Debug.Log("ENEMY SPAWNING ENDED");
        Debug.Log(enemyCount + " ENEMY IN GAME");
    }

    void Movement(float x, float y)
    {
        transform.position = new Vector3(x + speed, y);
        if (hitBorder)
        {
            ShiftEnemiesDown();
        }
    }

    public void ShiftEnemiesDown()
    {
        if (hitBorder)
        {
            Movementy(downStep);
            hitBorder = false;
            Debug.Log("ENEMIES SHIFTED DOWN");
        }
    }

    public void Movementy(float downStep)
    {
        float x = transform.position.x;
        float y = transform.position.y;

        speed = -speed;
        transform.position = new Vector3(x + speed*5, y + downStep, transform.position.z);
        Debug.Log(name.ToString() + " SHIFTED");
    }
}
