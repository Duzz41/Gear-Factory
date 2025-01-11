using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int factoryLevel = 0;
    public static GameManager instance;
    public SpawnPoint spawnPoint; // Spawn noktası
    public GameObject aiPrefab; // Yapay zeka prefab'ı
    public int maxAI = 2; // Maksimum AI sayısı
    public List<GameObject> coins = new List<GameObject>();
    public List<GameObject> gearTrashs = new List<GameObject>();
    public List<GameObject> attackableObjects = new List<GameObject>();
    public List<GameObject> humans = new List<GameObject>();
    public List<GameObject> constBuildings = new List<GameObject>();
    public ParticleSystem sandEffect;
    public bool _isDay;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        SpawnAI();
        //sandEffect.Play();
    }

    public void SpawnAI()
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
    public void OnConstructionStarted(Wall wall)
    {
        // İnşaat başladığında yapılacak işlemler
        // Örneğin, constBuildings listesine ekleyebilirsiniz
        if (!constBuildings.Contains(wall.gameObject))
            constBuildings.Add(wall.gameObject);
    }

    public void OnConstructionCompleted(Wall wall)
    {
        // İnşaat tamamlandığında yapılacak işlemler
        constBuildings.Remove(wall.gameObject);
    }
    public void SpawnHumans()
    {

    }
    private int CountAIAtSpawn(Transform spawn)
    {
        // Belirli bir spawn noktasındaki AI sayısını say
        return spawn.childCount;
    }
}
