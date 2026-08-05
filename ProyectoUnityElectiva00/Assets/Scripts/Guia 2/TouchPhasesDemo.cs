using UnityEngine;
using TMPro; // <-- Importante para usar TextMeshPro
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchPhasesDemo : MonoBehaviour
{
    [Header("Referencias UI (TextMeshPro)")]
    public TMP_Text txtPhase;
    public TMP_Text txtPosition;
    public TMP_Text txtDelta;
    public TMP_Text txtDirection;
    public TMP_Text txtMagnitude;
    public TMP_Text txtPressure;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        // Registro en consola
        foreach (var touch in Touch.activeTouches)
        {
            Debug.Log($"Finger {touch.finger.index} | Phase {touch.phase} | Pos {touch.screenPosition} | Delta {touch.delta}");
        }

        // Mostrar datos en los textos de pantalla
        if (Touch.activeTouches.Count > 0)
        {
            var touch = Touch.activeTouches[0];

            if (txtPhase) txtPhase.text = "Phase: " + touch.phase;
            if (txtPosition) txtPosition.text = "Position: " + touch.screenPosition;
            if (txtDelta) txtDelta.text = "Delta: " + touch.delta;

            Vector2 direction = touch.delta.normalized;
            float magnitude = touch.delta.magnitude;

            if (txtDirection) txtDirection.text = "Direction: " + direction.ToString("F2");
            if (txtMagnitude) txtMagnitude.text = "Magnitude: " + magnitude.ToString("F2");

            if (txtPressure)
            {
                txtPressure.text = (touch.pressure != 0) 
                    ? "Pressure: " + touch.pressure.ToString("F2") 
                    : "Pressure: pressure no disponible";
            }
        }
    }
}