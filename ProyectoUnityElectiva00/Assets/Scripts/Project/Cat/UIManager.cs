using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Esta línea permite usar los textos bonitos de TextMeshPro

public class UIManager : MonoBehaviour
{
    [Header("Paneles de la Interfaz")]
    public GameObject panelPausa;
    public GameObject panelGameOver;
    public GameObject panelVictoria; // Panel que se mostrará al ganar

    [Header("Textos del Panel de Victoria")]
    public TextMeshProUGUI txtTiempoVictoria;   // Mostrará los minutos y segundos
    public TextMeshProUGUI txtPuntajeVictoria;  // Mostrará la comida recolectada
    public TextMeshProUGUI txtVidasVictoria;    // Mostrará las vidas restantes

    // Variables internas para contar el tiempo
    private float tiempoTranscurrido = 0f;
    private bool nivelFinalizado = false;

    void Start()
    {
        // Al iniciar el juego, aseguramos que todos los menús estén apagados/ocultos
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(false);

        // El tiempo corre a velocidad normal (1) y el reloj empieza en 0
        Time.timeScale = 1f;
        tiempoTranscurrido = 0f;
        nivelFinalizado = false;
    }

    void Update()
    {
        // Mientras el nivel esté activo y no esté en pausa, el reloj suma segundos
        if (!nivelFinalizado && Time.timeScale > 0f)
        {
            tiempoTranscurrido += Time.deltaTime;
        }
    }

    // --- FUNCIÓN QUE ACTIVA LA VICTORIA ---
    public void MostrarVictoria(int puntajeFinal, int vidasRestantes)
    {
        nivelFinalizado = true; // Detenemos el conteo del reloj
        Time.timeScale = 0f;    // Congelamos el movimiento del juego

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true); // Mostramos el panel de victoria

            // 1. Calculamos minutos y segundos
            int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60);
            int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60);

            // 2. Escribimos los datos en la pantalla
            if (txtTiempoVictoria != null)
                txtTiempoVictoria.text = "Tiempo: " + minutos.ToString("00") + ":" + segundos.ToString("00");

            if (txtPuntajeVictoria != null)
                txtPuntajeVictoria.text = "Comida: " + puntajeFinal + " pts";

            if (txtVidasVictoria != null)
                txtVidasVictoria.text = "Vidas restantes: " + vidasRestantes;
        }
    }

    // --- FUNCIONES DE PAUSA ---
    public void PausarJuego()
    {
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f; // Congela el juego
    }

    public void ReanudarJuego()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f; // Descongela el juego
    }

    // --- FUNCIÓN DE GAME OVER (DERROTA) ---
    public void MostrarGameOver()
    {
        nivelFinalizado = true;
        if (panelGameOver != null) panelGameOver.SetActive(true);
        Time.timeScale = 0f;
    }

    // --- NAVEGACIÓN DE BOTONES ---
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}