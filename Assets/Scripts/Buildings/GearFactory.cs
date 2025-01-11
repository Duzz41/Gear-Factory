using System.Collections;
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
        level += 1;
        GameManager.instance.factoryLevel = level;
        Debug.Log("Level: " + level);
        if (level == 19)
        {
            price += 6;
        }
        else if (level == 20)
        {
            price = 0;
            //Kazanma ekranı
        }
        RedesignCoinPlaces();
    }

    public override void RedesignCoinPlaces()
    {
        base.RedesignCoinPlaces();
    }

    public override void BuyFrombuilding()
    {

        base.BuyFrombuilding();

    }


}
