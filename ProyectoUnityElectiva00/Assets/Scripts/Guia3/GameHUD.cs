using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("Referencias de Texto UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI counterText;

    [Header("Pantalla de Tutorial / Inicio")]
    public GameObject startTutorialPanel;
    public Image tutorialDisplayImage;
    public TextMeshProUGUI tutorialDescriptionText;
    public Sprite[] tutorialSprites; // Asigna aquí las 3 imágenes en Inspector
    [TextArea(2, 4)]
    public string[] tutorialDescriptions; // Explica cada paso en el Inspector
    public Button nextButton;
    public Button prevButton;
    public Button startButton;

    [Header("Pantalla de Victoria")]
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryTimeText;

    [Header("Pantalla de Pausa")]
    public GameObject pausePanel;

    [Header("Zona de Destino (Punto B)")]
    public Transform puntoB;
    public float targetRadius = 5.0f;

    private float elapsedTime = 0f;
    private bool isTimerRunning = false;
    private bool isCompleted = false;
    private bool isPaused = false;
    private bool isTutorialActive = true;

    private int currentStepIndex = 0;

    private void Start()
    {
        // 1. Pausar tiempo del juego mientras se lee el tutorial
        Time.timeScale = 0f;
        isTutorialActive = true;

        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // 2. Inicializar tutorial si está asignado
        if (startTutorialPanel != null)
        {
            startTutorialPanel.SetActive(true);
            startTutorialPanel.transform.SetAsLastSibling();
            UpdateTutorialUI();
        }
        else
        {
            // Si no hay tutorial, arrancar el tiempo normal
            Time.timeScale = 1f;
            isTutorialActive = false;
        }
    }

    private void Update()
    {
        if (isCompleted || isPaused || isTutorialActive) return;

        // Iniciar cronómetro tras el primer input del jugador
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

                if (rb != null && !rb.isKinematic)
                {
                    bool isInside = false;

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

    // --- LÓGICA DEL TUTORIAL DE INICIO ---

    private void UpdateTutorialUI()
    {
        if (tutorialSprites != null && tutorialSprites.Length > currentStepIndex)
        {
            if (tutorialDisplayImage != null)
                tutorialDisplayImage.sprite = tutorialSprites[currentStepIndex];
        }

        if (tutorialDescriptions != null && tutorialDescriptions.Length > currentStepIndex)
        {
            if (tutorialDescriptionText != null)
                tutorialDescriptionText.text = tutorialDescriptions[currentStepIndex];
        }

        // Control de visibilidad de botones en función de la diapositiva
        if (prevButton != null)
            prevButton.gameObject.SetActive(currentStepIndex > 0);

        bool isLastStep = (tutorialSprites != null && currentStepIndex == tutorialSprites.Length - 1);

        if (nextButton != null)
            nextButton.gameObject.SetActive(!isLastStep);

        if (startButton != null)
            startButton.gameObject.SetActive(isLastStep);
    }

    public void NextTutorialStep()
    {
        if (tutorialSprites != null && currentStepIndex < tutorialSprites.Length - 1)
        {
            currentStepIndex++;
            UpdateTutorialUI();
        }
    }

    public void PrevTutorialStep()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            UpdateTutorialUI();
        }
    }

    public void StartGameFromTutorial()
    {
        isTutorialActive = false;
        if (startTutorialPanel != null) startTutorialPanel.SetActive(false);
        Time.timeScale = 1f; // Reanudar tiempo del motor de física
    }

    // --- FUNCIONES PÚBLICAS PARA BOTONES DE PAUSA / REINICIO ---

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AlternarPausa()
    {
        if (isTutorialActive) return; // No pausar mientras se lee el tutorial inicial

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