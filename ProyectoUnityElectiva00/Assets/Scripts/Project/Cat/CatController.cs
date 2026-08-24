using UnityEngine;
using UnityEngine.InputSystem;

public class CatController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 8f;

    [Header("Mobile Jump Zone")]
    [Range(0f, 1f)]
    [SerializeField] private float jumpZoneWidth = 0.25f;

    [Range(0f, 1f)]
    [SerializeField] private float jumpZoneHeight = 0.30f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.15f;

    private Rigidbody2D rb;

    private InputAction moveAction;
    private InputAction jumpAction;

    private bool isGrounded;

    [SerializeField] private int maxJumps = 2;
    private int jumpsRemaining;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        var input = GetComponent<PlayerInput>();

        moveAction = input.actions["Move"];
        jumpAction = input.actions["Jump"];

        jumpsRemaining = maxJumps;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        rb.linearVelocity = new Vector2(
            input.x * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
        }

        if (jumpAction.WasPerformedThisFrame())
        {
            // Si el salto viene del teclado, funciona normalmente
            if (jumpAction.activeControl?.device is Keyboard)
            {
                TryJump();
            }
            // Si viene del móvil, solamente funciona
            // dentro de la zona invisible de salto
            else if (IsTouchInsideJumpZone())
            {
                TryJump();
            }
        }
    }

    private void TryJump()
    {
        if (jumpsRemaining <= 0)
            return;

        Jump();
        jumpsRemaining--;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );
    }

    private bool IsTouchInsideJumpZone()
    {
        if (Touchscreen.current == null)
            return false;

        Vector2 touchPosition =
            Touchscreen.current.primaryTouch.position.ReadValue();

        float zoneWidth = Screen.width * jumpZoneWidth;
        float zoneHeight = Screen.height * jumpZoneHeight;

        bool insideRight =
            touchPosition.x >= Screen.width - zoneWidth;

        bool insideBottom =
            touchPosition.y <= zoneHeight;

        return insideRight && insideBottom;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}