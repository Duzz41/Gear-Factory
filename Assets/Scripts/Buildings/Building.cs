using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Building : MonoBehaviour
{

    [HideInInspector] public bool lock_my_UI = false;

    public Canvas my_canvas;

    public List<Transform> coin_holders = new List<Transform>();
    public Transform content;
    public GameObject coin_place_prefab;

    public int price;

    // Temel binalar için ortak işlevler burada tanımlanabilir
    public virtual void Build()
    {
        //Debug.Log("Building constructed!");
    }

    public virtual void Upgrade()
    {
        // Debug.Log("Building upgraded!");

    }

    public virtual void BuyFrombuilding()
    {
        // Debug.Log("Building sold!");
    }

    public virtual void RemoveTool(Transform tool)
    {
        Destroy(tool.gameObject);

        // Debug.Log("Tool removed!");
    }

    public virtual void RedesignCoinPlaces()
    {
        // Debug.Log("Coin places redesigned!");
        if (content == null)
        {
            Debug.LogError("Content is not assigned!");
            return;
        }

        // Önce mevcut coin yerlerini temizle
        foreach (Transform coinHolder in coin_holders)
        {
            Destroy(coinHolder.gameObject);
        }
        coin_holders.Clear(); // Listeyi temizle

        // Yeni coin sayısını hesapla
        // int new_coins_count = price - content.childCount;


        for (int i = 0; i < price; i++)
        {
            GameObject added_coin_place = Instantiate(coin_place_prefab, content);
            coin_holders.Add(added_coin_place.transform);
        }


    }

}
