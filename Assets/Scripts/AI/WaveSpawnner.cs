using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class Wave
{
    public string waveName;
    public int noOfEnemies;
    public GameObject[] typeOfEnemies;
    public float spawnInterval;

}
public class WaveSpawnner : MonoBehaviour
{
    public Wave[] waves;
    public Transform[] spawnPoint;

    private Wave currentWave;
    private int currentWaveNumber = 0;
    bool canSpawn = true;
    float nextSpawnTime;
    void Update()
    {
        currentWave = waves[currentWaveNumber];
        SpawnWave();
        GameObject[] totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (totalEnemies.Length == 0 && !canSpawn && currentWaveNumber != waves.Length)
        {
            SpawnNextWave();
        }
    }


    void SpawnNextWave()
    {
        currentWaveNumber++;
        canSpawn = true;
    }
    void SpawnWave()
    {
        if (GameManager.instance._isDay == false)
        {
            if (canSpawn && nextSpawnTime < Time.time)
            {
                GameObject randomEnemy = currentWave.typeOfEnemies[Random.Range(0, currentWave.typeOfEnemies.Length)];
                Transform randomSpawnPoint = spawnPoint[Random.Range(0, spawnPoint.Length)];

                Instantiate(randomEnemy, randomSpawnPoint.position, Quaternion.identity);
                currentWave.noOfEnemies--;

                nextSpawnTime = Time.time + currentWave.spawnInterval;
                if (currentWave.noOfEnemies == 0)
                {
                    canSpawn = false;
                }
            }
        }
    }


}

