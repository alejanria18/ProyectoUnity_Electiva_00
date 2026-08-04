using UnityEngine;
using UnityEngine.Rendering;

public class ObjectController: MonoBehaviour
{
    public GameObject Avatar;

    public float moveStep = 0.25f;
    public float rotateStep = 15f;
    public float scaleStep = 0.1f;



    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;

    void Start()
    {
        if (Avatar != null)
        {
            // Guardamos la configuración inicial al comenzar
            initialPosition = Avatar.transform.position;
            initialRotation = Avatar.transform.rotation;
            initialScale = Avatar.transform.localScale;
        }
    }

    // Método para restablecer los parámetros
    public void ResetTransform()
    {
        if (Avatar != null)
        {
            Avatar.transform.position = initialPosition;
            Avatar.transform.rotation = initialRotation;
            Avatar.transform.localScale = initialScale;
        }
    }

    public void TranslateUp()
    {
        Avatar.transform.Translate(Vector3.up * moveStep);
    }

    public void TranslateDown()
    {
        Avatar.transform.Translate(Vector3.down * moveStep);
    }

    public void TranslateRight()
    {
        Avatar.transform.Translate(Vector3.right * moveStep);
    }

    public void Translateleft()
    {
        Avatar.transform.Translate(Vector3.left * moveStep);
    }

    public void Rotateleft()
    {
        Avatar.transform.Rotate(0f, 0f, -rotateStep);
    }

    public void RotateRight()
    {
        Avatar.transform.Rotate( 0f, 0f, rotateStep);
    }

    public void ScaleUp()
    {
        Avatar.transform.localScale += Vector3.one * scaleStep;
    }

    public void ScaleDown()
    {
        Avatar.transform.localScale -= Vector3.one * scaleStep;
    }
}
