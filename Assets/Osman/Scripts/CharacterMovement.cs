using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Karakterin hareket hızı
    public Camera cam;

    private Vector2 movementInput;
    private Rigidbody2D rb;
    private PlayerInputActions playerInputActions;
    public float interactionDistance = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInputActions = new PlayerInputActions(); // PlayerInputActions sınıfını oluşturuyoruz
    }

    private void FixedUpdate()
    {

        CharacterMove();
        cam.transform.position = new Vector3(transform.position.x, cam.transform.position.y, cam.transform.position.z);
    }

    private void CharacterMove()
    {
        // Karakterin sağa veya sola hareketi
        Vector2 move = new Vector2(movementInput.x * moveSpeed, rb.velocity.y);
        rb.velocity = move;
    }


    private void TestInteraction()
    {
        Clockwork_AI[] aiControllers = FindObjectsOfType<Clockwork_AI>();

        foreach (var ai in aiControllers)
        {
            // Yapay zeka ile mesafeyi hesapla
            float distance = Vector2.Distance(transform.position, ai.transform.position);

            // Eğer mesafe etkileşim mesafesinden kısa ise etkileşim metodunu çağır
            if (distance <= interactionDistance)
            {
                ai.Interact();
                ai.isBroken = false;
            }
        }
    }






    private void OnEnable()
    {
        // Input olaylarını kaydeder
        playerInputActions.Player.Enable();

        // Hareket girdisi olayı
        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMove;
        playerInputActions.Player.Interaction.performed += OnInteraction;

    }
    private void OnDisable()
    {
        // Input olaylarını devre dışı bırak
        playerInputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // Hareket girdisini al
        movementInput = context.ReadValue<Vector2>();
    }
    private void OnInteraction(InputAction.CallbackContext context)
    {
        // Etkilesim girdisini al
        TestInteraction();
    }

}
