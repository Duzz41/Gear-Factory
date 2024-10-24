using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SpawnPoint spawnPoint; // Spawn noktası
    public GameObject aiPrefab; // Yapay zeka prefab'ı
    public int maxAI = 2; // Maksimum AI sayısı

    private void Start()
    {
        SpawnAI();
    }

    private void SpawnAI()
    {
        // Tüm spawn noktalarında AI'ları doğur
        foreach (Transform spawn in spawnPoint.spawnPoints)
        {
            // Bu spawn noktasında zaten AI var mı kontrol et
            if (CountAIAtSpawn(spawn) < maxAI)
            {
                for (int i = 0; i < maxAI - 1; i++)
                {
                    GameObject ai = Instantiate(aiPrefab, spawn.position, Quaternion.identity);
                    ai.transform.parent = spawn;
                    ai.GetComponent<Clockwork_AI>().spawnPoint = spawn;
                }


            }
        }
    }

    private int CountAIAtSpawn(Transform spawn)
    {
        // Belirli bir spawn noktasındaki AI sayısını say
        return spawn.childCount;
    }
}
