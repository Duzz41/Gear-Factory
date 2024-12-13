using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Building
{
    public int health = 3;
    private int level = 0;



    public override void Build()
    {
        base.Build();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);//wall sprite'ını tutan obje

        level = 1;
        health = 3;
        price += 2;
        RedesignCoinPlaces();
        if (!GameManager.instance.attackableObjects.Contains(gameObject))
        {
            GameManager.instance.attackableObjects.Add(gameObject);
        }

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
            price += 2;             
            health = 5;
            //price değeri arttır
            RedesignCoinPlaces();
            //Sprite'ı Değiştir
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);

        }



    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            DestroyWall();
        }
    }

    private void DestroyWall()
    {
        if (GameManager.instance.attackableObjects.Contains(gameObject))
        {
            GameManager.instance.attackableObjects.Remove(gameObject);
        }

        // Duvarı yok et
        Debug.Log("Wall Destroyed!");
        Destroy(gameObject);
    }

    public override void RedesignCoinPlaces()
    {
        base.RedesignCoinPlaces();
    }

}
