using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    public Transform targetObject;
    public float speed = 5f;
    public TMP_Text txtVectorInfo;

    private InputAction moveAction;
    private InputAction tapAction;

    private void Awake()
    {
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        tapAction = playerInput.actions["Tap"];
    }

    private void OnEnable()
    {
        if (tapAction != null) tapAction.performed += OnTap;
    }

    private void OnDisable()
    {
        if (tapAction != null) tapAction.performed -= OnTap;
    }

    private void OnTap(InputAction.CallbackContext context)
    {
        Debug.Log("Tap ejecutado");
    }

    private void Update()
    {
        if (moveAction != null)
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
            if (moveInput.sqrMagnitude > 0.01f)
            {
                Debug.Log("MOVE RECIBIDO: " + moveInput);
            }
            if (targetObject != null)
            {
                targetObject.Translate(movement * speed * Time.deltaTime, Space.World);
            }

            if (txtVectorInfo != null)
            {
                txtVectorInfo.text = $"Move Vector: {moveInput}\nDirección: {moveInput.normalized}";
            }
        }
    }
}