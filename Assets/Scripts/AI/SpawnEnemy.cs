using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab; // Spawn edilecek düşman prefab
    public Transform[] spawnPoints; // Düşmanların spawn olacağı noktalar
    public float spawnInterval = 5f; // Spawn aralığı

    private float spawnTimer;

    void Update()
    {
        SpawnEnemies();
        // Gece olduğunda düşmanları spawn etmeye başla

    }

    void SpawnEnemies()
    {
        // Tüm spawn noktalarında düşman oluştur
        if (GameManager.instance._isDay == false)
        {
            spawnTimer += Time.deltaTime;

            // Belirli bir aralıkla spawn yap
            if (spawnTimer >= spawnInterval)
            {
                foreach (Transform spawnPoint in spawnPoints)
                {
                    Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                }
                spawnTimer = 0f; // Timer'ı sıfırla
            }
        }
    }
}
