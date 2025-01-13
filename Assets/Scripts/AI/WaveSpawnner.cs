using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



[System.Serializable]
public class Wave
{
    public int waveNumber;
    public int noOfEnemies;
    public GameObject[] typeOfEnemies;
    public float spawnInterval;

}
public class WaveSpawnner : MonoBehaviour
{
    public Wave[] waves;
    public Transform[] spawnPoint;
    //public Animator anim;


    private Wave currentWave;
    private int currentWaveNumber = 0;

    float nextSpawnTime;
    void Start()
    {
        currentWave = waves[currentWaveNumber];
    }
    void Update()
    {
        ;
        SpawnWave();
        Debug.Log("Başlangıçta mevcut dalga: " + currentWave.waveNumber + ", Düşman sayısı: " + currentWave.noOfEnemies);
    }


    void SpawnNextWave()
    {
        currentWaveNumber++;
        currentWave = waves[currentWaveNumber];
    }
    void SpawnWave()
    {
        if (GameManager.instance._isDay == false && currentWave.waveNumber == GameManager.instance._dayCount)
        {
            Debug.Log("Düşman doğurma kontrolü: Gün: " + GameManager.instance._dayCount + ", Mevcut dalga: " + currentWave.waveNumber);

            if (nextSpawnTime < Time.time)
            {
                if (currentWave.noOfEnemies > 0)
                {
                    GameObject randomEnemy = currentWave.typeOfEnemies[Random.Range(0, currentWave.typeOfEnemies.Length)];
                    Transform randomSpawnPoint = spawnPoint[Random.Range(0, spawnPoint.Length)];

                    GameObject human = Instantiate(randomEnemy, new Vector2(randomSpawnPoint.position.x, 1f), Quaternion.identity);
                    GameManager.instance.humans.Add(human);
                    currentWave.noOfEnemies--;

                    Debug.Log("Düşman doğuruldu: " + human.name + ", Kalan düşman sayısı: " + currentWave.noOfEnemies);

                    nextSpawnTime = Time.time + currentWave.spawnInterval;
                    if (currentWave.noOfEnemies == 0)
                    {
                        Debug.Log("Dalga tamamlandı: " + currentWave.waveNumber);
                        SpawnNextWave();
                    }
                }
            }
            else
            {
                Debug.Log("Düşman sayısı sıfır, yeni dalga başlatılıyor.");
            }
        }
        else
        {
            Debug.Log("Düşman doğurmak için uygun koşullar sağlanmadı.");
        }
    }


}

