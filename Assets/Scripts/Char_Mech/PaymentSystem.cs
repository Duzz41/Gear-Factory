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
    [SerializeField] private GameObject coin_prefab;
    [SerializeField] private float fill_speed;
    [SerializeField] private float drop_duration;
    [SerializeField] private float remove_duration;

    #region Coin Jobs
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
        if (current_slot < building_slot_count)
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
                PaymentDone();
            }
        }
    }
    void FillCoinForAI()
    {
        if (current_slot < 1)
        {
            Vector3 dropPos = transform.position;
            CoinCollect.instance.coin_count -= 1;
            CoinCollect.instance.RemoveToCoinFromList(null);
            GameObject coin = Instantiate(coin_prefab, dropPos + new Vector3(3f, 0, 0), Quaternion.identity);
            GameManager.instance.coins.Add(coin);

            coin.GetComponent<Rigidbody2D>().simulated = true;

        }
    }

    void PaymentDone()
    {
        Debug.Log("Payment Done");
    }
    IEnumerator RemoveCoinOnComplete()
    {

        CancelInvoke("FillCoin");
        yield return new WaitForSeconds(remove_duration);
        for (int i = active_coins.Count; i > 0; i--)
        {
            Destroy(active_coins[0]);
            active_coins.RemoveAt(0);
        }
        current_slot = 0;
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
    IEnumerator ControlBuildingUI(Collider2D other)
    {
        float maxWaitTime = 2f; // Örneğin 5 saniye bekliyoruz
        float startTime = Time.time;
        yield return new WaitUntil(() => active_coins.Count == 0 || Time.time - startTime >= maxWaitTime);
        if (active_coins.Count == 0 && !can_interact)
        {
            //if (other.transform.GetChild(0).gameObject != null)
            other.transform.GetChild(0).gameObject.SetActive(false);
        }

    }

    private Building building_cs;
    private GameObject targetObject;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Building")
        {
            OpenUI(other);
            #region GetObjectInfos
            targetObject = other.gameObject;
            building_cs = targetObject.GetComponent<Building>();
            robots = null; // Clear robots reference to avoid conflicts
            building_slot_count = building_cs.coin_holders.Count;
            // can_interact = true;
            #endregion
        }
        else if (other.gameObject.tag == "AI")
        {

            //OpenUI(other);
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
            can_interact = false;
            StartCoroutine(ControlBuildingUI(other));
        }
        else if (other.gameObject.tag == "AI")
        {
            can_interact = false;
        }


    }

    void OpenUI(Collider2D other)
    {
        other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        can_interact = true;

    }

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
            StartCoroutine(RemoveCoinOnComplete());
        }
        else if (active_coins.Count > 0)
        {
            StartCoroutine(DropCoin());
        }
    }
}
