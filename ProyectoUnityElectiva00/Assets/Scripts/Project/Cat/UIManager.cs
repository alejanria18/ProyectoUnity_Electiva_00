using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Paneles de la Interfaz")]
    public GameObject panelPausa;
    public GameObject panelGameOver;

    void Start()
    {
        // Esto asegura que al darle Play, los menús estén ocultos y el tiempo corra normal
        panelPausa.SetActive(false);
        panelGameOver.SetActive(false);
        Time.timeScale = 1f; 
    }

    // --- FUNCIONES DE PAUSA ---
    public void PausarJuego()
{
    Debug.Log("¡BOTON DE PAUSA PRESIONADO!");
    panelPausa.SetActive(true);
    Time.timeScale = 0f;
}

    public void ReanudarJuego()
    {
        panelPausa.SetActive(false);
        Time.timeScale = 1f; // Esto descongela el juego
    }

    // --- FUNCIONES DE GAME OVER ---
    public void MostrarGameOver()
    {
        panelGameOver.SetActive(true);
        Time.timeScale = 0f; // Congela el juego al morir
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f; // Volvemos el tiempo a la normalidad antes de reiniciar
        // Esto recarga la escena actual, no importa cómo la hayan llamado tus compañeros
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    // --- NAVEGACIÓN ---
    public void VolverAlMenuPrincipal()
    {
        Time.timeScale = 1f; // Volvemos el tiempo a la normalidad
        SceneManager.LoadScene("MainMenu"); // Si tu escena inicial se llama distinto, cambia "MainMenu" aquí
    }
}