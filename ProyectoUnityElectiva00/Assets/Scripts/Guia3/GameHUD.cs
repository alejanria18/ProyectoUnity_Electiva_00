using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("Referencias de Texto UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI counterText;

    [Header("Pantalla de Victoria")]
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryTimeText;

    [Header("Pantalla de Pausa")]
    public GameObject pausePanel;

    [Header("Zona de Destino (Punto B)")]
    public Transform puntoB;
    public float targetRadius = 5.0f; // Radio ampliado para detectar fácil

    private float elapsedTime = 0f;
    private bool isTimerRunning = false;
    private bool isCompleted = false;
    private bool isPaused = false;

    private void Start()
    {
        Time.timeScale = 1f;
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (isCompleted || isPaused) return;

        // Iniciar cronómetro
        if (!isTimerRunning)
        {
            bool touched = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
            bool clicked = Mouse.current != null && Mouse.current.leftButton.isPressed;
            bool keyPressed = Keyboard.current != null && Keyboard.current.anyKey.isPressed;

            if (touched || clicked || keyPressed)
            {
                isTimerRunning = true;
            }
        }

        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime % 60F);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100F) % 100F);

            if (timerText != null)
                timerText.text = string.Format("Tiempo: {0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
        }

        CheckCubesInPointB();
    }

    private void CheckCubesInPointB()
    {
        if (puntoB == null || isCompleted) return;

        int countInB = 0;
        string[] cubeNames = { "Cubo1", "Cubo2", "Cubo3", "Cubo4" };

        Collider puntoBCollider = puntoB.GetComponent<Collider>();

        foreach (string cubeName in cubeNames)
        {
            GameObject cube = GameObject.Find(cubeName);
            if (cube != null)
            {
                Rigidbody rb = cube.GetComponent<Rigidbody>();

                // REGLA CLAVE: Solo contar si el cubo ya fue SOLTADO (isKinematic == false).
                // Si el personaje lo lleva cargado en la mano (isKinematic == true), se ignora.
                if (rb != null && !rb.isKinematic)
                {
                    bool isInside = false;

                    // 1. Verificar si está dentro de los límites de la plataforma roja B
                    if (puntoBCollider != null)
                    {
                        Vector3 cPos = cube.transform.position;
                        Vector3 min = puntoBCollider.bounds.min;
                        Vector3 max = puntoBCollider.bounds.max;

                        if (cPos.x >= min.x - 0.2f && cPos.x <= max.x + 0.2f &&
                            cPos.z >= min.z - 0.2f && cPos.z <= max.z + 0.2f)
                        {
                            isInside = true;
                        }
                    }
                    else
                    {
                        // 2. Detección por distancia de respaldo
                        float dist = Vector3.Distance(new Vector3(cube.transform.position.x, 0, cube.transform.position.z), 
                                                      new Vector3(puntoB.position.x, 0, puntoB.position.z));
                        if (dist <= targetRadius)
                        {
                            isInside = true;
                        }
                    }

                    if (isInside)
                    {
                        countInB++;
                    }
                }
            }
        }

        if (counterText != null)
            counterText.text = $"Apilados: {countInB}/4";

        // ¡VICTORIA! Al haber 4 cubos soltados y apilados en Punto B
        if (countInB >= 4 && !isCompleted)
        {
            isCompleted = true;
            isTimerRunning = false;

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
                victoryPanel.transform.SetAsLastSibling();
            }

            if (victoryTimeText != null)
            {
                int minutes = Mathf.FloorToInt(elapsedTime / 60F);
                int seconds = Mathf.FloorToInt(elapsedTime % 60F);
                victoryTimeText.text = string.Format("¡RETO COMPLETADO!\nTiempo Final: {0:00}:{1:00}s", minutes, seconds);
            }
        }
    }

    // --- FUNCIONES PÚBLICAS PARA BOTONES ---

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AlternarPausa()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
                pausePanel.transform.SetAsLastSibling();
            }
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
        }
    }
}