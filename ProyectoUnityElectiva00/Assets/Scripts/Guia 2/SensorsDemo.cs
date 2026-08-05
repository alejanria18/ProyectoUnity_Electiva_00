using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Alias para corregir la ambigüedad de Gyroscope entre InputSystem y UnityEngine
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

public class SensorsDemo : MonoBehaviour
{
    [Header("Objeto en escena a mover/rotar")]
    public Transform targetObject;
    public float accelSpeed = 5f;
    public float gyroSpeed = 100f;

    [Header("UI Info")]
    public Text txtSensorInfo;

    private void OnEnable()
    {
        if (Accelerometer.current != null) InputSystem.EnableDevice(Accelerometer.current);
        if (Gyroscope.current != null) InputSystem.EnableDevice(Gyroscope.current);
    }

    private void Update()
    {
        Vector3 accel = Vector3.zero;
        Vector3 gyro = Vector3.zero;

        if (Accelerometer.current != null)
        {
            accel = Accelerometer.current.acceleration.ReadValue();
        }

        if (Gyroscope.current != null)
        {
            gyro = Gyroscope.current.angularVelocity.ReadValue();
        }

        // Registro en consola (Captura 4 de la guía)
        Debug.Log($"Accel: {accel} | Gyro: {gyro}");

        // Control del objeto 3D
        if (targetObject != null)
        {
            targetObject.Translate(new Vector3(accel.x, accel.y, 0) * accelSpeed * Time.deltaTime, Space.World);
            targetObject.Rotate(gyro * gyroSpeed * Time.deltaTime, Space.Self);
        }

        // Mostrar en UI
        if (txtSensorInfo != null)
        {
            txtSensorInfo.text = $"Accel: {accel}\nGyro: {gyro}";
        }
    }
}