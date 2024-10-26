using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearFactory : Building
{
    public override void Build()
    {
        base.Build();
        Debug.Log("Building-specific construction logic.");
    }

    public override void Upgrade()
    {
        base.Upgrade();
        Debug.Log("Building-specific upgrade logic.");
    }

    public override void Sell()
    {
        base.Sell();
        Debug.Log("Building-specific sell logic.");
    }
}
