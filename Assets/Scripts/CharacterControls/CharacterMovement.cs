using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.LowLevel;

public class CharacterMovement : MonoBehaviour
{

  [SerializeField] private Canvas mini_game;

  [Header("Movement")]
  [SerializeField] private float run_speed = 5f;
  [SerializeField] private float walk_speed = 2f;
  private float current_speed;
  public float energy = 20f;
  private float horizontal_input;


  [Header("Interaction")]
  private bool can_interact = false;
  private int current_slot = 0;
  private int building_slot_count;
  public List<GameObject> active_coins = new List<GameObject>();

  [SerializeField] private GameObject coin_prefab;
  [SerializeField] private float fill_speed;
  [SerializeField] private float drop_duration;
  [SerializeField] private float remove_duration;



  void Start()
  {

    current_speed = run_speed;
    EventDispatcher.RegisterFunction("MiniGameForEnergy", MiniGameForEnergy);
  }


  void Update()
  {


    UpdateEnergy();
    UpdateSpeed();



    transform.Translate(new Vector2(horizontal_input * current_speed * Time.deltaTime, 0));
  }

  #region Movement/Energy/Speed
  void MiniGameForEnergy()
  {
    mini_game.gameObject.SetActive(false);
  }
  void UpdateEnergy()
  {
    if (horizontal_input != 0)
    {
      if (current_speed != walk_speed)
      {
        energy -= 1 * Time.deltaTime;
      }
      else
      {
        energy += 0.5f * Time.deltaTime;
      }
    }
    else
    {
      energy += 1f * Time.deltaTime;
    }

    energy = Mathf.Clamp(energy, 0, 20);
  }
  void UpdateSpeed()
  {
    if (energy > 10)
    {
      current_speed = run_speed;
    }
    else if (energy <= 0)
    {
      current_speed = walk_speed;
      mini_game.gameObject.SetActive(true);
      EventDispatcher.SummonEvent("ActivateGame");
    }
  }

  #endregion

  #region Coin Jobs
  void FillCoin()
  {

    if (can_interact)
    {
      if (CoinCollect.instance.coin_count > 0)
      {
        if (current_slot < building_slot_count)
        {

          Vector3 slot_position = building_cs.coin_holders[current_slot].transform.position;
          current_slot += 1;
          CoinCollect.instance.coin_count -= 1;


          GameObject coin = Instantiate(coin_prefab, this.transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
          coin.transform.DOMove(slot_position, fill_speed).OnComplete(() =>
          {
            coin.GetComponent<Rigidbody2D>().simulated = false;
            CoinCollect.instance.RemoveToCoinFromList(null);
          });
          active_coins.Add(coin);

        }

        if (current_slot == building_slot_count)
        {
          StartCoroutine(RemoveCoinOnComplete());
          PaymentDone();
        }

      }

      if (CoinCollect.instance.coin_count == 0)
      {
        StartCoroutine(DropCoin());
      }

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
    if (active_coins.Count > 0)
    {

      for (int i = active_coins.Count; i > 0; i--)
      {

        yield return new WaitForSeconds(drop_duration);
        if (active_coins.Count != 0)
        {
          active_coins[0].GetComponent<Rigidbody2D>().simulated = true;
          active_coins.RemoveAt(0);
        }

      }
      current_slot = 0;
      StopCoroutine(DropCoin());
    }

  }

  #endregion
  IEnumerator ControlBuildingUI()
  {
    float maxWaitTime = 2f; // Örneğin 5 saniye bekliyoruz
    float startTime = Time.time;
    yield return new WaitUntil(() => active_coins.Count == 0 || Time.time - startTime >= maxWaitTime);
    if (active_coins.Count == 0 && !can_interact)
    {
      building_cs.transform.GetChild(0).gameObject.SetActive(false);
    }
    StopCoroutine(ControlBuildingUI());

  }

  private Building building_cs;

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.gameObject.tag == "Building")
    {
      other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
      can_interact = true;


      #region GetObjectInfos
      building_cs = other.gameObject.GetComponent<Building>();
      building_slot_count = building_cs.coin_holders.Count;
      #endregion
    }
  }
  void OnTriggerExit2D(Collider2D other)
  {
    if (other.gameObject.tag == "Building")
    {
      can_interact = false;
      StartCoroutine(ControlBuildingUI());
    }
  }





  #region Inputs
  public void WASD(InputAction.CallbackContext context)
  {
    horizontal_input = context.ReadValue<Vector2>().x;
  }

  public void InteractionKey(InputAction.CallbackContext context)
  {

    if (context.performed)
    {
      InvokeRepeating("FillCoin", 0.5f, 1f);
    }
    if (context.canceled)
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
  #endregion
}