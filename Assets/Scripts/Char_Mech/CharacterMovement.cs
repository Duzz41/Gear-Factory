
using UnityEngine;
using UnityEngine.InputSystem;

using Cinemachine;

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
  public MiniGame mini_game;
  public Animator anim;
  public float maxSpeed = 20f;
  [Header("Cinemachine")]
  [SerializeField] private CinemachineVirtualCamera cinemachineCam; // Cinemachine referansı
  [SerializeField] private float offsetWhenFacingRight = 2f;
  [SerializeField] private float offsetWhenFacingLeft = -2f;
  [SerializeField] private float offsetLerpSpeed = 5f;
  private CinemachineFramingTransposer transposer;
  private Vector3 originalOffset; // Store the original offset
  private float idleTimer = 0f; // Timer to track idle time
  private const float idleThreshold = 5f;



  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    current_speed = run_speed;
    AudioManager.instance.PlayCar("Car Idle");
    EventDispatcher.RegisterFunction("MiniGameForEnergy", MiniGameForEnergy);
    if (cinemachineCam != null)
    {
      transposer = cinemachineCam.GetCinemachineComponent<CinemachineFramingTransposer>();
      if (transposer != null)
      {
        originalOffset = transposer.m_TrackedObjectOffset; // Store the original offset
      }
    }
  }


  void FixedUpdate()
  {
    UpdateEnergy();
    UpdateSpeed();
    MovementParticules();
    Movement();
    UpdateCinemachineOffset();
  }



  #region Movement/Energy/Speed

  void Movement()
  {
    rb.velocity = new Vector2(horizontal_input * current_speed, rb.velocity.y);
    anim.SetFloat("speed", rb.velocity.magnitude);
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
        //Debug.Log("Movement Particle is Playing");
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
  private bool isCarMoving = false; // Arabanın hareket edip etmediğini kontrol etmek için bir bayrak

  void UpdateSpeed()
  {
    if (energy > 10)
    {
      current_speed = isTurbo ? turbo_speed : run_speed;
    }
    else if (energy <= 0)
    {
      current_speed = walk_speed;
      mini_game_canvas.gameObject.SetActive(true);
      EventDispatcher.SummonEvent("ActivateGame");
    }

    // Araba hareket ediyor mu kontrol et
    if (horizontal_input != 0)
    {
      UpdateEngineSound();
      if (!isCarMoving) // Eğer araba hareket etmiyorsa
      {
        AudioManager.instance.PlayCar("Car Move"); // Araba hareket sesi çal
        isCarMoving = true; // Bayrağı güncelle
      }
    }
    else
    {
      if (isCarMoving) // Eğer araba duruyorsa
      {
        AudioManager.instance.StopCar(); // Araba hareket sesini durdur
        AudioManager.instance.PlayCar("Car Idle");
        isCarMoving = false; // Bayrağı güncelle
      }
    }

  }
  void UpdateEngineSound()
  {
    // Calculate the pitch based on current speed
    float pitch = Mathf.Clamp(rb.velocity.magnitude / maxSpeed, 0.5f, 2f); // Adjust the range as needed
    AudioManager.instance.carSource.pitch = pitch;

  }
  void UpdateCinemachineOffset()
  {
    if (horizontal_input == 0)
    {
      idleTimer += Time.deltaTime; // Increment timer
      if (idleTimer >= idleThreshold)
      {
        // Change the offset after 5 seconds of being idle
        ChangeOffset();
      }
    }
    else
    {
      idleTimer = 0f; // Reset timer if moving
      ResetOffset(); // Reset the offset when moving
    }
  }

  void ChangeOffset()
  {
    if (cinemachineCam != null && transposer != null)
    {
      float targetOffsetX = facingRight ? offsetWhenFacingRight : offsetWhenFacingLeft;
      float newOffsetX = Mathf.Lerp(transposer.m_TrackedObjectOffset.x, targetOffsetX, offsetLerpSpeed * Time.deltaTime);
      transposer.m_TrackedObjectOffset = new Vector3(newOffsetX, transposer.m_TrackedObjectOffset.y, transposer.m_TrackedObjectOffset.z);
    }
  }

  void ResetOffset()
  {
    if (cinemachineCam != null && transposer != null)
    {
      transposer.m_TrackedObjectOffset = originalOffset; // Reset to the original offset
    }
  }

  #endregion






  #region Inputs
  public void WASD(InputAction.CallbackContext context)
  {
    horizontal_input = context.ReadValue<Vector2>().x;
  }
  public void Energy(InputAction.CallbackContext context)
  {
    if (context.performed)
    {
      mini_game.MiniGameForEnergy();
    }
  }
  private bool isTurbo = false;
  public void Shift(InputAction.CallbackContext context)
  {
    isTurbo = context.performed;
  }


  #endregion
}