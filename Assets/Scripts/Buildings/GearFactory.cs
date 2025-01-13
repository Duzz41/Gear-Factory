using System.Collections;
using TMPro;
using UnityEngine;
public class GearFactory : Building
{
    private int level = 1;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] Animator finalAnim;

    void Start()
    {
        levelText.text = level.ToString() + " / 20";
    }

    public override void Build()
    {
        base.Build();
        // Debug.Log("Building-specific construction logic.");
    }

    public override void Upgrade()
    {
        base.Upgrade();
        level++;
        levelText.text = level.ToString() + " / 20";
        GameManager.instance.factoryLevel = level;
        Debug.Log("Level: " + level);
        if (level == 19)
        {
            price += 6;
        }
        else if (level == 20)
        {
            finalAnim.SetTrigger("PlayFinal");
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
