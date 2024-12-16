using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.LowLevel;

public class CharacterMovement : MonoBehaviour
{

  [SerializeField] private Canvas mini_game_canvas;
  private Rigidbody2D rb;

  [Header("Movement")]
  [SerializeField] private float turbo_speed;
  [SerializeField] private float run_speed = 5f;
  [SerializeField] private float walk_speed = 2f;
  private float current_speed;
  public float energy = 20f;
  private float horizontal_input;
  public ParticleSystem movementParticle, frontTire;
  private bool facingRight = true;
  public GameObject carSprite;






  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
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
    mini_game_canvas.gameObject.SetActive(false);
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
      if (current_speed == run_speed)
      {
        energy -= 1 * Time.deltaTime;

      }
      else if (current_speed == turbo_speed)
      {
        energy -= 1.5f * Time.deltaTime;
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
  private bool energy_renew = false;
  void UpdateSpeed()
  {

    if (energy > 10)
    {
      if (energy_renew) energy_renew = false;
      current_speed = isTurbo ? turbo_speed : run_speed;
    }
    else if (energy <= 0)
    {
      energy_renew = true;
      current_speed = walk_speed;
      mini_game_canvas.gameObject.SetActive(true);
      EventDispatcher.SummonEvent("ActivateGame");
    }
    else if (!energy_renew && !isTurbo)
    {
      current_speed = run_speed;
    }




  }

  #endregion






  #region Inputs
  public void WASD(InputAction.CallbackContext context)
  {
    horizontal_input = context.ReadValue<Vector2>().x;
  }

  private bool isTurbo = false;
  public void Shift(InputAction.CallbackContext context)
  {
    if (context.started) isTurbo = true;
    if (context.canceled) isTurbo = false;
  }


  #endregion
}