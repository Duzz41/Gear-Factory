using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public Animator anim;
    public TMP_Text waveNameText;


    private Wave currentWave;
    private int currentWaveNumber = 0;

    float nextSpawnTime;
    bool canSpawn = true;
    bool canAnimate = false;
    void Update()
    {
        currentWave = waves[currentWaveNumber];
        SpawnWave();
        GameObject[] totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (totalEnemies.Length == 0)
        {
            if (currentWaveNumber + 1 != waves.Length)
            {
                if (canAnimate)
                {
                    Debug.Log("Wave Complete");
                    waveNameText.text = waves[currentWaveNumber + 1].waveName;
                    anim.Play("WaveStart");
                    canAnimate = false;
                }
            }
            else
            {
                Debug.Log("Game Over");
            }
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

                GameObject human = Instantiate(randomEnemy, new Vector2(randomSpawnPoint.position.x, 1f), Quaternion.identity);
                GameManager.instance.humans.Add(human);
                currentWave.noOfEnemies--;

                nextSpawnTime = Time.time + currentWave.spawnInterval;
                if (currentWave.noOfEnemies == 0)
                {
                    canSpawn = false;
                    canAnimate = true;
                }
            }
        }
    }


}

