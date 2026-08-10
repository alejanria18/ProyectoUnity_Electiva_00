using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInputReader : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Movimiento")]
    [SerializeField] private Transform targetObject;
    [SerializeField] private float speed = 5f;

    [Header("Interfaz")]
    [SerializeField] private TMP_Text txtVectorInfo;

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    private void Update()
    {
        if (moveAction == null)
            return;

        // Leer directamente Player/Move
        Vector2 moveInput =
            moveAction.action.ReadValue<Vector2>();

        // Movimiento en X e Y
        Vector3 movement = new Vector3(
            moveInput.x,
            moveInput.y,
            0f
        );

        if (targetObject != null)
        {
            targetObject.position +=
                movement *
                speed *
                Time.deltaTime;
        }

        // Dirección normalizada
        Vector2 normalizedDirection =
            moveInput.sqrMagnitude > 0.001f
            ? moveInput.normalized
            : Vector2.zero;

        // Mostrar información
        if (txtVectorInfo != null)
        {
            txtVectorInfo.text =
                $"Move Vector: ({moveInput.x:F2}, {moveInput.y:F2})\n" +
                $"Dirección: ({normalizedDirection.x:F2}, {normalizedDirection.y:F2})";
        }
    }
}