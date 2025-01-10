using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PaymentSystem : MonoBehaviour
{
    [Header("Interaction")]
    private bool can_interact = false;
    private int current_slot = 0;
    private int building_slot_count;
    public List<GameObject> active_coins = new List<GameObject>();
    private Clockwork_AI robots;
    [SerializeField] bool isCoinsFalling = false;
    [SerializeField] bool isCoinsRemoving = false;
    [SerializeField] private GameObject coin_prefab;
    [SerializeField] private float fill_speed;
    [SerializeField] private float drop_duration;
    [SerializeField] private float remove_duration;
    [SerializeField] GameObject carSprite;

    #region Coin Jobs
    private Building building_cs;
    private GameObject targetObject;



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Building")
        {
            #region GetObjectInfos
            targetObject = other.gameObject;
            building_cs = targetObject.GetComponent<Building>();
            robots = null; //Clear robots reference to avoid conflicts
            if (building_cs != null)
            {
                building_slot_count = building_cs.price;

                if (!building_cs.lock_my_UI)
                {
                    OpenUI(building_cs);
                }
            }
            #endregion
        }
        else if (other.gameObject.tag == "AI")
        {
            #region GetAIInfos
            targetObject = other.gameObject;
            robots = targetObject.GetComponent<Clockwork_AI>();
            building_cs = null; // Clear building reference to avoid conflicts
            can_interact = true;
            #endregion
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Building")
        {

            var test = other.gameObject.GetComponent<Building>();
            if (building_cs == test)
            { can_interact = false; }
            Debug.Log(test.name);
            StartCor(test);
        }
        else if (other.gameObject.tag == "AI")
        {
            can_interact = false;
        }


    }
    void StartCor(Building test)
    {
        if (gameObject.activeInHierarchy) // GameObject aktif mi?
        {
            StartCoroutine(ControlBuildingUI(test));
        }

    }
    void FillCoin()
    {
        if (can_interact)
        {
            if (CoinCollect.instance.coin_count > 0)
            {
                // Determine whether to use Building or AI for coin filling
                if (building_cs != null)
                {
                    if (isCoinsFalling == false)
                        FillCoinForBuilding();
                }
                else if (robots != null)
                {
                    FillCoinForAI();
                }
            }
            if (CoinCollect.instance.coin_count == 0)
            {
                StartCoroutine(DropCoin());
            }
        }
    }
    void FillCoinForBuilding()
    {
        if (current_slot < building_slot_count && building_cs != null)
        {
            Vector3 slot_position = building_cs.coin_holders[current_slot].transform.position;
            current_slot += 1;

            CoinCollect.instance.coin_count -= 1;

            GameObject coin = Instantiate(coin_prefab, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
            coin.transform.DOMove(slot_position, fill_speed).OnComplete(() =>
            {
                coin.GetComponent<Rigidbody2D>().simulated = false;
                CoinCollect.instance.RemoveToCoinFromList(null);

            });

            active_coins.Add(coin);

            if (current_slot == building_slot_count)
            {
                StartCoroutine(RemoveCoinOnComplete());

            }
        }
    }
    void FillCoinForAI()
    {
        if (current_slot < 1)
        {
            Vector3 dropPos = transform.position;

            // Karakterin bakış yönünü al
            float lookDirection = carSprite.transform.localScale.x; // Eğer karakter sağa bakıyorsa, bu doğru olur. 
                                                                    // Eğer karakterin bakış yönü farklı bir eksende ise, uygun yönü kullanmalısınız.

            // Coin'in konumunu bakış yönüne göre hesapla
            Vector3 coinPosition = new Vector3(dropPos.x + lookDirection * 3f, dropPos.y, dropPos.z); // 3 birim ileri

            CoinCollect.instance.coin_count -= 1;
            CoinCollect.instance.RemoveToCoinFromList(null);
            GameObject coin = Instantiate(coin_prefab, coinPosition, Quaternion.identity);
            GameManager.instance.coins.Add(coin);

            coin.GetComponent<Rigidbody2D>().simulated = true;

        }
    }

    void PaymentDone()
    {
        Debug.Log("Payment Done");
        Debug.Log(targetObject.name);
        targetObject.GetComponent<Building>().Upgrade();
    }
    IEnumerator RemoveCoinOnComplete()
    {
        Debug.Log("naağğber müdür");
        isCoinsRemoving = true;
        CancelInvoke("FillCoin");
        yield return new WaitForSeconds(remove_duration);

        for (int i = active_coins.Count; i > 0; i--)
        {
            Destroy(active_coins[0]);
            active_coins.RemoveAt(0);
        }
        current_slot = 0;
        isCoinsRemoving = false;
        PaymentDone();
        building_slot_count = building_cs.price;  //Binanın price değeri değiştiği için bu değeri tekrar güncelliyoruz
        StopCoroutine(RemoveCoinOnComplete());
    }
    IEnumerator DropCoin()
    {
        CancelInvoke("FillCoin");
        isCoinsFalling = true;
        if (active_coins.Count > 0)
        {

            for (int i = active_coins.Count; i > 0; i--)
            {

                yield return new WaitForSeconds(drop_duration);
                if (active_coins.Count != 0)
                {
                    active_coins[0].GetComponent<Rigidbody2D>().simulated = true;
                    GameManager.instance.coins.Add(active_coins[0]);
                    active_coins.RemoveAt(0);
                }

            }
            current_slot = 0;
            isCoinsFalling = false;
            StopCoroutine(DropCoin());
        }

    }


    #endregion
    IEnumerator ControlBuildingUI(Building test)
    {
        float maxWaitTime = 2f; // Örneğin 5 saniye bekliyoruz
        float startTime = Time.time;
        yield return new WaitUntil(() => active_coins.Count == 0 || Time.time - startTime >= maxWaitTime);
        if (active_coins.Count == 0)
        {
            //if (other.transform.GetChild(0).gameObject != null)
            // other.transform.GetChild(0).gameObject.SetActive(false);
            if (test != null)
            {
                test.my_canvas.gameObject.SetActive(false);
                //Debug.Log(test.name);
            }

        }

    }



    void OpenUI(Building building)
    {
        building.my_canvas.gameObject.SetActive(true);
        can_interact = true;
    }

    #region Input Actions

    public void InteractionKey(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            InvokeRepeating("FillCoin", 0.5f, 1f);

        }
        if (context.canceled)
        {
            CancelAction();
        }
    }
    public void CancelAction()
    {
        CancelInvoke("FillCoin");

        if (active_coins.Count == building_slot_count)
        {
            if (!isCoinsRemoving)
            { StartCoroutine(RemoveCoinOnComplete()); }
        }
        else if (active_coins.Count > 0)
        {
            StartCoroutine(DropCoin());

        }
    }
    #endregion
}
