using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Building : MonoBehaviour
{
    [SerializeField] private BuildingsSO buildingsSO;

    [SerializeField] private Canvas my_canvas;

    public List<Transform> coin_holders = new List<Transform>();

    public int price = 3;

    // Temel binalar için ortak işlevler burada tanımlanabilir
    public virtual void Build()
    {
        Debug.Log("Building constructed!");
    }

    public virtual void Upgrade()
    {
        Debug.Log("Building upgraded!");
    }

    public virtual void Sell()
    {
        Debug.Log("Building sold!");
    }

}
