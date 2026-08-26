using UnityEngine;

public class CatController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Joystick")]
    [SerializeField] private JoyStickMover joystick;

    [Header("Animacion")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckGround();
        UpdateAnimations();
        FlipCat();
    }

    private void FixedUpdate()
    {
        Move();

    }

    private void Move()
    {
        if (joystick == null)
            return;

        float horizontal = joystick.Horizontal;

        rb.linearVelocity = new Vector2(
            horizontal * moveSpeed,
            rb.linearVelocity.y
        );
    }

    public void Jump()
    {
     

        if (!isGrounded)
            return;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );

     
    }

   

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void UpdateAnimations()
    {
        if (animator == null)
            return;

        animator.SetFloat(
            "Speed",
            Mathf.Abs(joystick.Horizontal)
        );

        animator.SetBool(
            "IsGrounded",
            isGrounded
        );

        animator.SetFloat(
            "YVelocity",
            rb.linearVelocity.y
        );
    }
    private void FlipCat()
    {
        if (spriteRenderer == null)
            return;

        if (joystick.Horizontal > 0.05f)
        {
            // Derecha
            spriteRenderer.flipX = true;
        }
        else if (joystick.Horizontal < -0.05f)
        {
            // Izquierda
            spriteRenderer.flipX = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }
    }
}