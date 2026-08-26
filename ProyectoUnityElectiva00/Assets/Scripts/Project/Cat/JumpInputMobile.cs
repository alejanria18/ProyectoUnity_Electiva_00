using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class JumpInputMobile : MonoBehaviour
{
    [SerializeField] private CatController catController;

    private void OnEnable()
    {
        if (!EnhancedTouchSupport.enabled)
            EnhancedTouchSupport.Enable();
    }

    private void Update()
    {
        HandleMouse();
        HandleTouch();
    }

    // ==========================================
    // MOUSE --- prueba en unity
    // ==========================================

    private void HandleMouse()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition =
                Mouse.current.position.ReadValue();

            // Solo mitad derecha de pantalla
            if (mousePosition.x > Screen.width / 2f)
            {
                // No saltar si estamos tocando un botón UI
                if (!IsPointerOverButton(mousePosition))
                {
                    catController.Jump();
                }
            }
        }
    }

    // ==========================================
    // TOUCH 
    // ==========================================

    private void HandleTouch()
    {
        foreach (Touch touch in Touch.activeTouches)
        {
            if (touch.phase ==
                UnityEngine.InputSystem.TouchPhase.Began)
            {
                Vector2 position = touch.screenPosition;

                // Mitad derecha
                if (position.x > Screen.width / 2f)
                {
                    if (!IsPointerOverButton(position))
                    {
                        catController.Jump();
                    }
                }
            }
        }
    }

    // ==========================================
    // EVITAR SALTAR AL TOCAR PAUSA / POWER UP
    // ==========================================

    private bool IsPointerOverButton(Vector2 position)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData pointerData =
            new PointerEventData(EventSystem.current);

        pointerData.position = position;

        List<RaycastResult> results =
            new List<RaycastResult>();

        EventSystem.current.RaycastAll(
            pointerData,
            results
        );

        foreach (RaycastResult result in results)
        {
            if (result.gameObject
                .GetComponentInParent<Button>() != null)
            {
                return true;
            }
        }

        return false;
    }
}