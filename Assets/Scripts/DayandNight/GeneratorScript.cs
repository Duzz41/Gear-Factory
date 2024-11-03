using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
    [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    GameObject[] clouds;

    [SerializeField]
    float spawnInterval;

    [SerializeField]
    GameObject endPoint;


    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        Prewarm();
        Invoke("AttemptSpawn", spawnInterval);
    }

    void SpawnCloud(Vector3 startPos)
    {
        int randomIndex = Random.Range(0, clouds.Length);
        GameObject cloud = Instantiate(clouds[randomIndex]);

        float startY = Random.Range(startPos.y - 2f, startPos.y + 1f);
        cloud.transform.position = new Vector3(startPos.x, startY, startPos.z);

        float scale = Random.Range(0.8f, 1.3f);
        cloud.transform.localScale = new Vector2(scale, scale);

        float speed = Random.Range(0.1f, 0.5f);
        cloud.GetComponent<CloudScript>().StartFloating(speed);
    }
    void AttemptSpawn()
    {
        startPos = transform.position;
        SpawnCloud(startPos);
        Invoke("AttemptSpawn", spawnInterval);
    }

    void Prewarm()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 spawnPos = startPos + Vector3.right * (i * 2);
            SpawnCloud(spawnPos);
        }
    }
}
