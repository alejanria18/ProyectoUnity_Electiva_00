using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

using Gyroscope = UnityEngine.InputSystem.Gyroscope;

public class SensorsDemo : MonoBehaviour
{
    [Header("Objeto")]
    public Transform targetObject;

    [Header("Sensibilidad")]
    public float accelSpeed = 2f;
    public float gyroSpeed = 1f;

    [Header("Suavizado")]
    public float smoothFactor = 5f;

    [Header("Zona muerta")]
    public float deadZone = 0.15f;

    [Header("Limites")]
    public float maxX = 4f;
    public float maxY = 5f;

    [Header("UI")]
    public TMP_Text txtSensorInfo;

    private Vector3 smoothAccel = Vector3.zero;
    private Vector3 smoothGyro = Vector3.zero;

    private Vector3 accelNeutral;
    private Vector3 initialPosition;

    private bool calibrated = false;
    private bool hasAccelerometer = false;
    private bool hasGyroscope = false;

    private void OnEnable()
    {
        if (targetObject == null)
            targetObject = transform;

        initialPosition = targetObject.position;

        // ACELEROMETRO
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            hasAccelerometer = true;
        }
        else
        {
            hasAccelerometer = false;
        }

        // GIROSCOPIO
        if (Gyroscope.current != null)
        {
            InputSystem.EnableDevice(Gyroscope.current);
            hasGyroscope = true;
        }
        else
        {
            hasGyroscope = false;
        }

        calibrated = false;
    }

    private void Update()
    {
        Vector3 accel = Vector3.zero;
        Vector3 gyro = Vector3.zero;

        // -----------------------
        // LEER ACELEROMETRO
        // -----------------------

        if (Accelerometer.current != null)
        {
            accel =
                Accelerometer.current.acceleration.ReadValue();

            hasAccelerometer = true;
        }

        // -----------------------
        // LEER GIROSCOPIO
        // -----------------------

        if (Gyroscope.current != null)
        {
            gyro =
                Gyroscope.current.angularVelocity.ReadValue();

            hasGyroscope = true;
        }

        // -----------------------
        // CALIBRACION INICIAL
        // -----------------------

        if (!calibrated && hasAccelerometer)
        {
            accelNeutral = accel;
            smoothAccel = accel;

            calibrated = true;
        }

        // -----------------------
        // SUAVIZADO
        // -----------------------

        float smooth =
            1f - Mathf.Exp(-smoothFactor * Time.deltaTime);

        smoothAccel =
            Vector3.Lerp(
                smoothAccel,
                accel,
                smooth
            );

        smoothGyro =
            Vector3.Lerp(
                smoothGyro,
                gyro,
                smooth
            );

        // Quitar posición inicial del teléfono
        Vector3 relativeAccel =
            smoothAccel - accelNeutral;

        // Zona muerta para evitar pequeños temblores
        float inputX = relativeAccel.x;
        float inputY = relativeAccel.y;

        if (Mathf.Abs(inputX) < deadZone)
            inputX = 0f;

        if (Mathf.Abs(inputY) < deadZone)
            inputY = 0f;

        // -----------------------
        // MOVIMIENTO X / Y
        // -----------------------

        if (targetObject != null)
        {
            Vector3 movement =
                new Vector3(
                    inputX,
                    inputY,
                    0f
                );

            targetObject.Translate(
                movement *
                accelSpeed *
                Time.deltaTime,
                Space.World
            );

            // Mantenerlo dentro de pantalla
            Vector3 pos = targetObject.position;

            pos.x = Mathf.Clamp(
                pos.x,
                initialPosition.x - maxX,
                initialPosition.x + maxX
            );

            pos.y = Mathf.Clamp(
                pos.y,
                initialPosition.y - maxY,
                initialPosition.y + maxY
            );

            targetObject.position = pos;

            // -----------------------
            // ROTACION - GIROSCOPIO
            // -----------------------

            Vector3 rotation =
                smoothGyro *
                Mathf.Rad2Deg *
                gyroSpeed *
                Time.deltaTime;

            targetObject.Rotate(
                rotation,
                Space.Self
            );
        }

        // -----------------------
        // INFORMACION UI
        // -----------------------

        if (txtSensorInfo != null)
        {
            txtSensorInfo.text =
                $"ACELEROMETRO: {(hasAccelerometer ? "Disponible" : "No disponible")}\n" +
                $"X: {accel.x:F2}  Y: {accel.y:F2}  Z: {accel.z:F2}\n\n" +

                $"GIROSCOPIO: {(hasGyroscope ? "Disponible" : "No disponible")}\n" +
                $"X: {gyro.x:F2}  Y: {gyro.y:F2}  Z: {gyro.z:F2}";
        }
    }

    private void OnDisable()
    {
        // IMPORTANTE:
        // apagar sensores al salir de esta escena

        if (Accelerometer.current != null)
        {
            InputSystem.DisableDevice(
                Accelerometer.current
            );
        }

        if (Gyroscope.current != null)
        {
            InputSystem.DisableDevice(
                Gyroscope.current
            );
        }
    }
}