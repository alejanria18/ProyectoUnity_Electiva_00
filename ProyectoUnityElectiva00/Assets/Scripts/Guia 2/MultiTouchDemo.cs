using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MultiTouchDemo : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text txtTouchCount;
    public TMP_Text txtFingersInfo;
    public TMP_Text txtPinchDistance;

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
        int count = Touch.activeTouches.Count;
        if (txtTouchCount) txtTouchCount.text = "Toques activos: " + count;

        string info = "";
        for (int i = 0; i < count; i++)
        {
            var t = Touch.activeTouches[i];
            info += $"Finger {t.finger.index} (ID {t.touchId}): {t.screenPosition}\n";
        }
        if (txtFingersInfo) txtFingersInfo.text = info;

        // Código idéntico a la Captura 3 de la guía para distancia entre 2 dedos
        if (Touch.activeTouches.Count < 2) 
        {
            if (txtPinchDistance) txtPinchDistance.text = "Distancia: Requiere 2 dedos";
            return;
        }

        var first = Touch.activeTouches[0];
        var second = Touch.activeTouches[1];

        float distance = Vector2.Distance(first.screenPosition, second.screenPosition);
        Debug.Log($"Distancia entre dedos: {distance}");

        if (txtPinchDistance) txtPinchDistance.text = "Distancia entre dedos: " + distance.ToString("F2") + " px";
    }
}