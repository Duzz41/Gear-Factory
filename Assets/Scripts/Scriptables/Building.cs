using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Building : MonoBehaviour
{

    [HideInInspector] public bool lock_my_UI = false;

    public Canvas my_canvas;

    public List<Transform> coin_holders = new List<Transform>();
    public Transform content;
    public GameObject coin_place_prefab;

    public int price = 3;

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
        // Debug.Log("Tool removed!");
    }

}
