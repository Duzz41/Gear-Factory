using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Building
{
    public int health = 0;
    private int level = 0;
    private bool isConstructed = false;
    public GameObject constructionSprite; // İnşaatta olan sprite
    public GameObject[] builtSprite;
    public Transform walls;
    [SerializeField] GameObject destroyedWall;

    public override void Build()
    {
        base.Build();
        constructionSprite.SetActive(true);
        for (int i = 0; i < builtSprite.Length; i++)
            builtSprite[i].SetActive(false);
        level = 1;
        health = 10;
        price += 2;
        RedesignCoinPlaces();

        transform.parent = walls;
    }

    public override void Upgrade()
    {
        NotifyWorkerAboutConstruction();
        if (level == 0)
        {
            Build();
        }
        else
        {
            level += 1;
            price += 2;
            health += 5;
            //price değeri arttır
            RedesignCoinPlaces();
            //Sprite'ı Değiştir
            constructionSprite.SetActive(true);
            for (int i = 0; i < builtSprite.Length; i++)
                builtSprite[i].SetActive(false);

        }



    }
    private void NotifyWorkerAboutConstruction()
    {
        // İşçiyi bilgilendirmek için bir olay veya callback kullanabilirsiniz
        // Örneğin, GameManager'da bir olay tetikleyebilirsiniz
        GameManager.instance.OnConstructionStarted(this);
        isConstructed = true;
    }

    public void CompleteConstruction()
    {
        if (isConstructed == true)
        {
            GameManager.instance.OnConstructionCompleted(this);
            if (!GameManager.instance.attackableObjects.Contains(gameObject))
            {
                GameManager.instance.attackableObjects.Add(gameObject);
            }
            // İnşaat tamamlandığında yapılacak işlemler
            constructionSprite.SetActive(false); // İnşaatta olan sprite'ı gizle
            builtSprite[level - 1].SetActive(true);
            Debug.Log("Construction Complete!");
            isConstructed = false;
        }
    }

    public void TakeDamage(int damage)
    {
        AudioManager.instance.PlaySfx("Trash");
        health -= damage;
        if (health <= 0)
        {
            AudioManager.instance.PlaySfx("WallHit");
            DestroyWall();
        }
    }

    private void DestroyWall()
    {
        if (GameManager.instance.attackableObjects.Contains(gameObject))
        {

            GameManager.instance.attackableObjects.Remove(gameObject);
        }
        /*
        level = 0;
        health = 3;
        price = 1;
        constructionSprite.SetActive(true);
        for (int i = 0; i < builtSprite.Length; i++)
            builtSprite[i].SetActive(false);
        */
        //destroyedWall = gameObject;

        // Duvarı yok et

        GameObject NewWall = Instantiate(destroyedWall, destroyedWall.transform.position, Quaternion.identity);

        NewWall.transform.parent = destroyedWall.transform.parent;
        Wall NewWallcss = NewWall.GetComponent<Wall>();

        NewWallcss.level = 0;
        NewWallcss.health = 3;
        NewWallcss.price = 1;
        NewWallcss.constructionSprite.SetActive(true);
        for (int i = 0; i < builtSprite.Length; i++)
            NewWallcss.builtSprite[i].SetActive(false);
        RedesignCoinPlaces();
        Destroy(gameObject);

        // En basit haliyle yeniden oluştur

        // Duvarı yok et
        Debug.Log("Wall Destroyed!");
        //Destroy(gameObject);
    }

    public override void RedesignCoinPlaces()
    {
        base.RedesignCoinPlaces();
    }

}
