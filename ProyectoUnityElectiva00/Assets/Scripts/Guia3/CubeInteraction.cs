using UnityEngine;

public class CubeInteraction : MonoBehaviour
{
    [Header("Configuración de Cámara y Puntos")]
    public Camera playerCamera;
    public Transform holdPoint;
    public float reachDistance = 10.0f; // Mayor alcance para agarrar fácil

    private Rigidbody heldObject;

    private void Update()
    {
        // Mover suavemente el cubo tomado hacia el HoldPoint
        if (heldObject != null && holdPoint != null)
        {
            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, holdPoint.position, Time.deltaTime * 12f);
            heldObject.transform.rotation = Quaternion.Lerp(heldObject.transform.rotation, holdPoint.rotation, Time.deltaTime * 12f);
        }
    }

    public void TomarCubo()
    {
        if (heldObject != null) return; // Ya tenemos un cubo agarrado

        if (playerCamera == null) playerCamera = Camera.main;

        // Lanzar rayo desde el punto de mira (centro de pantalla)
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance))
        {
            Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();
            if (rb == null) rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                heldObject = rb;
                heldObject.isKinematic = true; // Desactivar física mientras lo llevamos
            }
        }
    }

    public void SoltarCubo()
    {
        if (heldObject != null)
        {
            heldObject.isKinematic = false; // Reactivar física y gravedad para apilar
            heldObject = null;
        }
    }
}