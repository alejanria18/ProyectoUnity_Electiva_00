using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class FPSPlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 5.0f;
    public CharacterController controller;

    [Header("Configuración de Cámara")]
    public Transform cameraTransform;
    public float lookSensitivity = 0.15f;

    [Header("Input Actions")]
    public InputActionReference moveAction;

    private float cameraPitch = 0f;

    private void OnEnable()
    {
        // Activar el soporte táctil mejorado del New Input System (Indispensable para celulares)
        EnhancedTouchSupport.Enable();
        if (moveAction != null && moveAction.action != null) moveAction.action.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        // 👇 LÍNEA AGREGADA: Si el juego se pausa (Time.timeScale == 0), no mueve personaje ni cámara
        if (Time.timeScale == 0f) return;

        MovePlayer();
        RotateCamera();
    }

    private void MovePlayer()
    {
        Vector2 input = Vector2.zero;

        // Leer del Joystick Virtual
        if (moveAction != null && moveAction.action != null)
            input = moveAction.action.ReadValue<Vector2>();

        if (input == Vector2.zero && Gamepad.current != null)
            input = Gamepad.current.leftStick.ReadValue();

        // Mover personaje en el mundo 3D
        if (input != Vector2.zero && controller != null)
        {
            Vector3 move = transform.right * input.x + transform.forward * input.y;
            move.y = -9.81f * Time.deltaTime; // Gravedad
            controller.Move(move * moveSpeed * Time.deltaTime);
        }
    }

    private void RotateCamera()
    {
        Vector2 lookDelta = Vector2.zero;

        // 1. CELULAR (PANTALLA TÁCTIL)
        // Detecta cualquier dedo que toque y arrastre en la mitad derecha de la pantalla
        if (Touch.activeTouches.Count > 0)
        {
            foreach (var touch in Touch.activeTouches)
            {
                // Si el toque ocurre a la derecha del 35% de la pantalla (zona libre para mirar)
                if (touch.screenPosition.x > Screen.width * 0.35f)
                {
                    lookDelta = touch.delta;
                    break;
                }
            }
        }

        // 2. COMPUTADORA / EDITOR (MOUSE)
        // Arrastrar el mouse en el lado derecho de la pantalla del juego
        if (lookDelta == Vector2.zero && Mouse.current != null)
        {
            if (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                if (mousePos.x > Screen.width * 0.35f)
                {
                    lookDelta = Mouse.current.delta.ReadValue();
                }
            }
        }

        // 3. APLICAR ROTACIÓN
        if (lookDelta != Vector2.zero)
        {
            // Girar el personaje a los lados (Eje Y)
            transform.Rotate(Vector3.up * lookDelta.x * lookSensitivity);

            // Girar la cámara arriba y abajo (Eje X)
            if (cameraTransform != null)
            {
                cameraPitch -= lookDelta.y * lookSensitivity;
                cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
                cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
            }
        }
    }
}