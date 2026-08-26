using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class JoyStickMover : MonoBehaviour
{
    [Header("Joystick")]
    [SerializeField] private RectTransform handle;
    [SerializeField] private float movementRange = 70f;

    public float Horizontal { get; private set; }

    private RectTransform area;
    private Canvas canvas;
    private Camera uiCamera;

    private Vector2 handleStartPosition;

    private bool mouseDragging = false;
    private int activeTouchId = -1;

    private void Awake()
    {
        area = GetComponent<RectTransform>();

        canvas = GetComponentInParent<Canvas>();

        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCamera = canvas.worldCamera;
        else
            uiCamera = null;

        if (handle != null)
            handleStartPosition = handle.anchoredPosition;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        if (EnhancedTouchSupport.enabled)
            EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        HandleMouse();
        HandleTouch();
    }

    // ========================================
    // MOUSE - PARA PROBAR EN UNITY
    // ========================================

    private void HandleMouse()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                area,
                mousePosition,
                uiCamera))
            {
                mouseDragging = true;

                UpdateJoystick(mousePosition);
            }
        }

        if (mouseDragging &&
            Mouse.current.leftButton.isPressed)
        {
            UpdateJoystick(mousePosition);
        }

        if (mouseDragging &&
            Mouse.current.leftButton.wasReleasedThisFrame)
        {
            mouseDragging = false;

            ResetJoystick();
        }
    }

    // ========================================
    // TOUCH - PARA ANDROID
    // ========================================

    private void HandleTouch()
    {
        foreach (Touch touch in Touch.activeTouches)
        {
            // Buscar un dedo que comience dentro del joystick
            if (activeTouchId == -1 &&
                touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(
                    area,
                    touch.screenPosition,
                    uiCamera))
                {
                    activeTouchId = touch.touchId;

                    UpdateJoystick(touch.screenPosition);
                }
            }

            // Solo seguimos el dedo que inició el joystick
            if (touch.touchId == activeTouchId)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                    touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    UpdateJoystick(touch.screenPosition);
                }

                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                    touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                {
                    activeTouchId = -1;

                    ResetJoystick();
                }
            }
        }
    }

    // ========================================
    // MOVIMIENTO DEL JOYSTICK
    // ========================================

    private void UpdateJoystick(Vector2 screenPosition)
    {
        if (handle == null)
            return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            area,
            screenPosition,
            uiCamera,
            out Vector2 localPoint))
        {
            float centerX =
                area.rect.xMin +
                (area.rect.width / 2f);

            float normalizedX =
                (localPoint.x - centerX) /
                (area.rect.width / 2f);

            normalizedX =
                Mathf.Clamp(normalizedX, -1f, 1f);

            // Pequeña zona muerta
            if (Mathf.Abs(normalizedX) < 0.08f)
                normalizedX = 0f;

            Horizontal = normalizedX;

            handle.anchoredPosition =
                handleStartPosition +
                new Vector2(
                    Horizontal * movementRange,
                    0
                );
        }
    }

    private void ResetJoystick()
    {
        Horizontal = 0f;

        if (handle != null)
            handle.anchoredPosition =
                handleStartPosition;
    }
}