using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class CoinCollect : MonoBehaviour
{
    public static CoinCollect instance;
    [SerializeField] private GameObject coinforbag_prefab;
    [SerializeField] private Transform spawn_point;
    [SerializeField] private SpriteRenderer front_bag;
    private float timer = 3f;
    public int coin_count = 0;

    public List<GameObject> coins = new List<GameObject>();


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
    }

    void ThrowBag()
    {
        front_bag.DOFade(0, 0.2f);


        GameObject new_coin = Instantiate(coinforbag_prefab, spawn_point.position, Quaternion.Euler(0, 90, 0));

        coins.Add(new_coin);

        coin_count += 1;

    }

    public void RemoveToCoinFromList(GameObject trash)
    {
        if (trash != null)
        {
            Destroy(trash);
            coins.Remove(trash);
            
            coin_count -= 1;
        }
        else
        {
            Destroy(coins[0]);
            coins.RemoveAt(0);

        }
    }

    public IEnumerator FadeFront()
    {
        yield return new WaitForSeconds(3f);
        if (timer < 0)
            front_bag.DOFade(1, 1.2f);
        StopCoroutine(FadeFront());
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            timer = 3f;
            Destroy(other.gameObject);
            GameManager.instance.coins.Remove(other.gameObject);
            AudioManager.instance.PlaySfx("Coin Collect");
            ThrowBag();
            StartCoroutine(FadeFront());
        }
    }

}
