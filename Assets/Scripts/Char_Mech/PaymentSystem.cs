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


    [SerializeField] private bool isInBuildingTrigger = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Building")
        {
            isInBuildingTrigger = true; // Building tetikleyicisine girdik
            targetObject = other.gameObject;
            building_cs = targetObject.GetComponent<Building>();
            robots = null; // Clear robots reference to avoid conflicts
            if (building_cs != null)
            {
                building_slot_count = building_cs.price;

                if (!building_cs.lock_my_UI)
                {
                    OpenUI(building_cs);
                }
            }
        }
        else if (other.gameObject.tag == "AI")
        {
            if (isInBuildingTrigger == false) // Eğer bir bina tetikleyicisindeysek
            {
                // AI ile etkileşimde bulunma
                targetObject = other.gameObject;
                robots = targetObject.GetComponent<Clockwork_AI>();
                can_interact = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Building")
        {
            isInBuildingTrigger = false; // Building tetikleyicisinden çıktık
            var test = other.gameObject.GetComponent<Building>();
            if (building_cs == test)
            {

                can_interact = false;
                if (active_coins.Count > 0)
                {
                    StartCoroutine(DropCoin());
                }
                StartCor(test);
            }
        }
        else if (other.gameObject.tag == "AI")
        {
            if (isInBuildingTrigger) // Eğer bir bina tetikleyicisindeysek
            {
                // AI ile etkileşimde bulunma
                can_interact = false;
            }
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
                    Debug.Log("Building cs not null");
                    //if (isCoinsFalling == false)
                        FillCoinForBuilding();
                }
                else if (robots != null)
                {
                    Debug.Log("Robot cs not null");
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
            active_coins.Add(coin);

            coin.transform.DOMove(slot_position, fill_speed).OnComplete(() =>
            {
                coin.GetComponent<Rigidbody2D>().simulated = false;
                CoinCollect.instance.RemoveToCoinFromList(null);


            });

            if (can_interact == true)
            {
                if (current_slot == building_slot_count)
                {
                    StartCoroutine(RemoveCoinOnComplete());
                }
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
        isCoinsFalling = false;
        targetObject.GetComponent<Building>().Upgrade();
    }
    IEnumerator RemoveCoinOnComplete()
    {
        isCoinsRemoving = true;
        CancelInvoke("FillCoin");
        yield return new WaitForSeconds(remove_duration);

        // Eğer active_coins listesi boşsa, işlemi durdur
        if (active_coins.Count == 0)
        {
            isCoinsRemoving = false;
            yield break; // Coroutine'i sonlandır
        }

        for (int i = active_coins.Count - 1; i >= 0; i--)
        {
            if (active_coins[i] != null) // Eğer nesne null değilse
            {
                Destroy(active_coins[i]);
            }
            active_coins.RemoveAt(i);
        }

        current_slot = 0;
        isCoinsRemoving = false;

        // Eğer building_cs null değilse, ödeme işlemini tamamla
        if (building_cs != null)
        {
            PaymentDone();
            building_slot_count = building_cs.price;  // Binanın price değeri değiştiği için bu değeri tekrar güncelliyoruz
        }
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
                    Debug.Log("düşşşş");
                    active_coins[0].GetComponent<Rigidbody2D>().simulated = true;
                    GameManager.instance.coins.Add(active_coins[0]);
                    active_coins.RemoveAt(0);
                    isCoinsFalling = false;
                }

            }
            current_slot = 0;
            
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
                building_cs = null;
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
            Debug.Log("Interaction Key Pressed");
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

        // Eğer aktif paralar varsa, DropCoin coroutine'ini başlat
        if (active_coins.Count > 0)
        {
            StartCoroutine(DropCoin());
        }
        else if (active_coins.Count == building_slot_count)
        {
            if (!isCoinsRemoving)
            {
                StartCoroutine(DropCoin());
            }
        }
    }
    #endregion
}