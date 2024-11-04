using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.LowLevel;

public class CharacterMovement : MonoBehaviour
{

  [SerializeField] private Canvas mini_game;
  private Rigidbody2D rb;
  PaymentSystem _paymentSystem;

  [Header("Movement")]
  [SerializeField] private float run_speed = 5f;
  [SerializeField] private float walk_speed = 2f;
  private float current_speed;
  public float energy = 20f;
  private float horizontal_input;
  public ParticleSystem movementParticle, frontTire;
  private bool facingRight = true;
  public GameObject carSprite;


  /* [Header("Interaction")]
   private bool can_interact = false;
   private int current_slot = 0;
   private int building_slot_count;
   public List<GameObject> active_coins = new List<GameObject>();
   private Clockwork_AI robots;
   [SerializeField] bool isCoinsFalling = false;
   [SerializeField] private GameObject coin_prefab;
   [SerializeField] private float fill_speed;
   [SerializeField] private float drop_duration;
   [SerializeField] private float remove_duration;*/



  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    _paymentSystem = GetComponent<PaymentSystem>();
    current_speed = run_speed;
    EventDispatcher.RegisterFunction("MiniGameForEnergy", MiniGameForEnergy);
  }


  void FixedUpdate()
  {
    UpdateEnergy();
    UpdateSpeed();
    MovementParticules();
    Movement();
  }



  #region Movement/Energy/Speed

  void Movement()
  {
    rb.velocity = new Vector2(horizontal_input * current_speed, rb.velocity.y);
    if (horizontal_input > 0 && !facingRight)
    {
      Flip();
    }
    else if (horizontal_input < 0 && facingRight)
    {
      Flip();
    }
  }
  void Flip()
  {
    facingRight = !facingRight;
    Vector3 scale = carSprite.transform.localScale;
    scale.x *= -1; // X eksenini tersine çevir
    carSprite.transform.localScale = scale;
  }
  void MiniGameForEnergy()
  {
    mini_game.gameObject.SetActive(false);
  }
  void MovementParticules()
  {
    if (rb.velocity.magnitude > 0.1f)
    {
      if (movementParticle.isPlaying == false)
      {
        movementParticle.Play();
        frontTire.Play();
        Debug.Log("Movement Particle is Playing");
      }

    }
    else
    {
      if (movementParticle.isPlaying == true)
      {
        movementParticle.Stop();
        frontTire.Stop();
      }
    }
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

  /* #region Coin Jobs
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

       GameObject coin = Instantiate(coin_prefab, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
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
     if (current_slot < building_slot_count)
     {
       Vector3 dropPos = transform.position;
       CoinCollect.instance.coin_count -= 1;

       GameObject coin = Instantiate(coin_prefab, dropPos + new Vector3(1.5f, 0, 0), Quaternion.identity);

       coin.GetComponent<Rigidbody2D>().simulated = true;
       CoinCollect.instance.RemoveToCoinFromList(null);
       active_coins.Add(coin);


       StartCoroutine(RemoveCoinOnComplete());
       PaymentDone();

     }
   }

   void PaymentDone()
   {
     Debug.Log("Payment Done");
   }
   GameObject targetCoin;
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
   void DropGear()
   {
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
       other.transform.GetChild(0).gameObject.SetActive(false);
     }
     StopCoroutine(ControlBuildingUI(other));

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
       #endregion
     }
     else if (other.gameObject.tag == "AI")
     {

       OpenUI(other);
       #region GetAIInfos
       targetObject = other.gameObject;
       robots = targetObject.GetComponent<Clockwork_AI>();
       if (robots.currentState == RobotState.Broken)
       {

       }
       building_cs = null; // Clear building reference to avoid conflicts
       building_slot_count = robots.coin_holders.Count;
       #endregion
     }
   }
   void OnTriggerExit2D(Collider2D other)
   {
     if (other.gameObject.tag == "Building" || other.gameObject.tag == "AI")
     {
       can_interact = false;
       StartCoroutine(ControlBuildingUI(other));
     }


   }

   void OpenUI(Collider2D other)
   {
     other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
     can_interact = true;

   }*/





  #region Inputs
  public void WASD(InputAction.CallbackContext context)
  {
    horizontal_input = context.ReadValue<Vector2>().x;
  }


  public MiniGame _miniGame;
  public void EnergyButton(InputAction.CallbackContext context)
  {
    if (context.started)
      _miniGame.MiniGameForEnergy();
  }
  #endregion
}