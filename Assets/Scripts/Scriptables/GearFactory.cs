using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class GearFactory : Building
{

    private int level = 1;


    public override void Build()
    {
        base.Build();
        // Debug.Log("Building-specific construction logic.");
    }

    public override void Upgrade()
    {
        base.Upgrade();
        //Debug.Log("Building-specific upgrade logic.");
        level += 1;
        price += 2;
        RedesignCoinPlaces();

    }

    private void RedesignCoinPlaces()
    {
        int new_coins_count = price - content.childCount;

        for (int i = 0; i < new_coins_count; i++)
        {
            GameObject added_coin_place = Instantiate(coin_place_prefab, content);
            coin_holders.Add(added_coin_place.transform);

        }
    }

    public override void BuyFrombuilding()
    {

        base.BuyFrombuilding();

    }


}
