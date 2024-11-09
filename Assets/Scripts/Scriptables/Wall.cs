using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Building
{
    private int health = 100;
    private int level = 0;



    public override void Build()
    {
        base.Build();
        transform.GetChild(0).gameObject.SetActive(true);//wall sprite'ını tutan obje
        level = 1;
        health = 100;
        price += 2;
        RedesignCoinPlaces();


    }

    public override void Upgrade()
    {
        if (level == 0)
        {
            Build();
        }
        else
        {
            level += 1;
            health = 100;
            //price değeri arttır
            RedesignCoinPlaces();

            //Sprite'ı Değiştir

        }



    }

    public override void RedesignCoinPlaces()
    {
        base.RedesignCoinPlaces();
    }

}
