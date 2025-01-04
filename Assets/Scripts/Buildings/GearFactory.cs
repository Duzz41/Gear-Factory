using System.Collections;

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

    public override void RedesignCoinPlaces()
    {
        base.RedesignCoinPlaces();
    }

    public override void BuyFrombuilding()
    {

        base.BuyFrombuilding();

    }


}
