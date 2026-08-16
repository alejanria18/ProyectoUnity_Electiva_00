using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CubeInteraction : MonoBehaviour
{
    [Header("Configuración de Cámara y Puntos")]
    public Camera playerCamera;
    public Transform holdPoint;
    public float reachDistance = 10.0f;

    [Header("Feedback Visual de Mira")]
    public Image reticleImage; // Arrastra aquí la imagen de la mira (Reticula)
    public Color normalColor = Color.red;
    public Color targetColor = Color.green;

    private Rigidbody heldObject;
    private Collider heldCollider;
    private Vector3 originalScale = Vector3.one;
    private GameObject activeGhost;
    private bool isAnimating = false;

    private void Start()
    {
        // Crear el Cubo Fantasma guía
        activeGhost = GameObject.CreatePrimitive(PrimitiveType.Cube);
        activeGhost.name = "GhostPreview";
        
        Collider ghostCol = activeGhost.GetComponent<Collider>();
        if (ghostCol != null) Destroy(ghostCol);

        Material ghostMat = new Material(Shader.Find("Sprites/Default"));
        ghostMat.color = new Color(0f, 1f, 0.3f, 0.45f); // Verde transparente
        activeGhost.GetComponent<Renderer>().material = ghostMat;
        activeGhost.SetActive(false);
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (activeGhost != null) activeGhost.SetActive(false);
            return;
        }

        // 1. Cambiar color de la mira a VERDE cuando apunta a un cubo
        CheckReticleHighlight();

        // 2. Sostener el cubo en la mano
        if (heldObject != null && holdPoint != null && !isAnimating)
        {
            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, holdPoint.position, Time.deltaTime * 18f);
            heldObject.transform.rotation = Quaternion.Slerp(heldObject.transform.rotation, holdPoint.rotation, Time.deltaTime * 18f);

            UpdatePlacementPreview();
        }
        else
        {
            if (activeGhost != null) activeGhost.SetActive(false);
        }
    }

    private void CheckReticleHighlight()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (reticleImage == null) return;

        if (heldObject != null)
        {
            reticleImage.color = normalColor;
            return;
        }

        // Raycast grueso para que detectar el cubo sea facilísimo
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.SphereCast(ray, 0.3f, out RaycastHit hit, reachDistance))
        {
            if (hit.collider.CompareTag("Interactable") || hit.collider.GetComponent<Rigidbody>() != null)
            {
                reticleImage.color = targetColor; // ¡VERDE si apunta a un cubo!
                return;
            }
        }

        reticleImage.color = normalColor; // ROJO si apunta al aire
    }

    private void UpdatePlacementPreview()
    {
        if (playerCamera == null) playerCamera = Camera.main;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.SphereCast(ray, 0.2f, out RaycastHit hit, reachDistance))
        {
            activeGhost.SetActive(true);
            Vector3 targetPos = GetSnappedPosition(hit);
            activeGhost.transform.position = targetPos;
            activeGhost.transform.rotation = Quaternion.identity;
            activeGhost.transform.localScale = originalScale;
        }
        else
        {
            activeGhost.SetActive(false);
        }
    }

    private Vector3 GetSnappedPosition(RaycastHit hit)
    {
        Vector3 targetPos = hit.point + (hit.normal * (originalScale.y * 0.5f));

        // Imán magnético: encaja exactamente en el centro (X, Z) del cubo de abajo
        if (hit.collider.CompareTag("Interactable") || hit.collider.GetComponent<Rigidbody>() != null)
        {
            Vector3 targetCubeCenter = hit.collider.bounds.center;
            float topY = hit.collider.bounds.max.y + (originalScale.y * 0.5f);
            targetPos = new Vector3(targetCubeCenter.x, topY, targetCubeCenter.z);
        }

        return targetPos;
    }

    public void TomarCubo()
    {
        if (Time.timeScale == 0f || heldObject != null || isAnimating) return;

        if (playerCamera == null) playerCamera = Camera.main;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.SphereCast(ray, 0.4f, out RaycastHit hit, reachDistance))
        {
            Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();
            if (rb == null) rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                heldObject = rb;
                heldCollider = hit.collider;
                originalScale = heldObject.transform.localScale;

                heldObject.isKinematic = true;
                if (heldCollider != null) heldCollider.enabled = false;

                heldObject.transform.localScale = originalScale * 0.4f;
            }
        }
    }

    public void SoltarCubo()
    {
        if (Time.timeScale == 0f || heldObject == null || isAnimating) return;

        if (playerCamera == null) playerCamera = Camera.main;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPosition;

        if (Physics.SphereCast(ray, 0.3f, out RaycastHit hit, reachDistance))
        {
            targetPosition = GetSnappedPosition(hit);
        }
        else
        {
            targetPosition = playerCamera.transform.position + playerCamera.transform.forward * 2.5f;
        }

        if (activeGhost != null) activeGhost.SetActive(false);

        StartCoroutine(AnimarColocacionCubo(targetPosition));
    }

    private IEnumerator AnimarColocacionCubo(Vector3 targetPos)
    {
        isAnimating = true;

        Vector3 startPos = heldObject.transform.position;
        Vector3 startScale = heldObject.transform.localScale;
        Quaternion startRot = heldObject.transform.rotation;
        Quaternion targetRot = Quaternion.identity;

        float duration = 0.18f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            heldObject.transform.position = Vector3.Lerp(startPos, targetPos, t);
            heldObject.transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            heldObject.transform.rotation = Quaternion.Lerp(startRot, targetRot, t);

            yield return null;
        }

        heldObject.transform.position = targetPos;
        heldObject.transform.localScale = originalScale;
        heldObject.transform.rotation = targetRot;

        if (heldCollider != null) heldCollider.enabled = true;

        heldObject.isKinematic = false;
        heldObject.constraints = RigidbodyConstraints.FreezeRotation;

        heldObject = null;
        heldCollider = null;
        isAnimating = false;
    }
}