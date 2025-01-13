using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int factoryLevel = 0;
    public static GameManager instance;
    public SpawnPoint trashSpawnPoint;
    public SpawnPoint spawnPoint; // Spawn noktası
    public GameObject aiPrefab; // Yapay zeka prefab'ı
    public GameObject trashPrefab;
    public int maxAI = 2; // Maksimum AI sayısı
    public List<GameObject> coins = new List<GameObject>();
    public List<GameObject> gearTrashs = new List<GameObject>();
    public List<GameObject> attackableObjects = new List<GameObject>();
    public List<GameObject> humans = new List<GameObject>();
    public List<GameObject> constBuildings = new List<GameObject>();
    public ParticleSystem sandEffect;
    public bool _isDay;
    public int _dayCount = 0;
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
        SpawnTrashs();
        InvokeRepeating("SetSimGear", 5f, 5f);
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
    public void SetSimGears()
    {
        foreach (GameObject gear in coins)
        {
            Rigidbody2D rb = gear.GetComponent<Rigidbody2D>();
            if (rb != null && rb.isKinematic == false)
            {
                // Coroutine'i başlat
                StartCoroutine(SetKinematicAfterDelay(rb, 5f));
            }
        }
    }
    private IEnumerator SetKinematicAfterDelay(Rigidbody2D rb, float delay)
    {
        // Belirtilen süre kadar bekle
        yield return new WaitForSeconds(delay);

        // isKinematic değerini true yap
        rb.isKinematic = true;
    }
    private int CountAIAtSpawn(Transform spawn)
    {
        // Belirli bir spawn noktasındaki AI sayısını say
        return spawn.childCount;
    }
    /// <summary>
    /// Spawnlar aracılığıyla çöpleri doğurur.
    /// </summary>
    public void SpawnTrashs()
    {
        // Spawn noktalarında çöpleri doğurma
        foreach (Transform spawn in trashSpawnPoint.spawnPoints)
        {
            Debug.Log("bisdfşkspfş");
            // Bu spawn noktasında zaten çöp var mı kontrol et
            if (CountTrashAtSpawn(spawn) < 2)
            {
                // Çöpleri doğurma
                for (int i = 0; i < maxAI - CountTrashAtSpawn(spawn); i++)
                {
                    GameObject trash = Instantiate(trashPrefab, spawn.position, Quaternion.identity);

                    trash.transform.parent = spawn;
                }
            }
        }
    }

    /// <summary>
    /// Belirli bir spawn noktasındaki çöp sayısını sayar.
    /// </summary>
    /// <param name="spawn">Spawn noktası</param>
    /// <returns>Çöp sayısı</returns>
    private int CountTrashAtSpawn(Transform spawn)
    {
        // Belirli bir spawn noktasındaki çöp sayısını say
        return spawn.childCount;

    }
}
