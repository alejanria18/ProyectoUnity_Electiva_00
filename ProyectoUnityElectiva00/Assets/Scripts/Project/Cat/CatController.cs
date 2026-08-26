using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // Para controlar la barra (Slider)
using TMPro;          // Para controlar el texto de vidas

public class CatController : MonoBehaviour
{
    [Header("Conexión con la Interfaz (UI)")]
    public UIManager uiManager;

    [Header("HUD Superior en Pantalla")]
    public Slider barraComida;               // La barrita de comida
    public TextMeshProUGUI txtContadorVidas; // El texto de vidas arriba

    [Header("Estadísticas del Gato")]
    public int vidas = 3;
    public int comidaRecolectada = 0;
    public int metaComida = 10;

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

    private void Start()
    {
        // Configuramos la barra de comida con la meta del nivel
        if (barraComida != null)
        {
            barraComida.minValue = 0;
            barraComida.maxValue = metaComida;
            barraComida.value = comidaRecolectada;
        }

        ActualizarHUD();
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
            if (jumpAction.activeControl?.device is Keyboard)
            {
                TryJump();
            }
            else if (IsTouchInsideJumpZone())
            {
                TryJump();
            }
        }

        // --- TECLAS DE PRUEBA EN PC ---
        // Presiona 'C' para simular comer comida (+1 punto)
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            SumarComida(1);
        }

        // Presiona 'X' para simular recibir daño (-1 vida)
        if (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame)
        {
            RecibirDano(1);
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

    // --- ACTUALIZAR LA PANTALLA ---
    private void ActualizarHUD()
    {
        if (barraComida != null)
        {
            barraComida.value = comidaRecolectada;
        }

        if (txtContadorVidas != null)
        {
            txtContadorVidas.text = "Vidas: " + vidas;
        }
    }

    // Sumar comida y verificar victoria
    public void SumarComida(int puntos)
    {
        comidaRecolectada += puntos;
        ActualizarHUD();

        if (comidaRecolectada >= metaComida)
        {
            if (uiManager != null)
            {
                uiManager.MostrarVictoria(comidaRecolectada, vidas);
            }
        }
    }

    // Recibir daño y verificar derrota
    public void RecibirDano(int cantidad)
    {
        vidas -= cantidad;
        if (vidas < 0) vidas = 0;
        
        ActualizarHUD();

        if (vidas <= 0)
        {
            if (uiManager != null)
            {
                uiManager.MostrarGameOver();
            }
        }
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