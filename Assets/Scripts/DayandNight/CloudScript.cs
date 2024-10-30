using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour
{
    private float _speed = 2;
    private GameObject _endPosX;

    // Start is called before the first frame update
    void Start()
    {
        _endPosX = GameObject.Find("EndPos");
    }
    public void StartFloating(float speed)
    {
        _speed = speed;

    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * (Time.deltaTime * _speed));
        if (transform.position.x > _endPosX.transform.position.x)
        {
            Debug.Log("naber");
            Destroy(gameObject);
        }
    }
}
