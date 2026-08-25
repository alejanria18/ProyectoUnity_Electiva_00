using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escenas

public class MainMenuManager : MonoBehaviour
{
    public void JugarJuego()
    {
        // Asegúrate de que el nombre coincida exactamente con la escena de tus compañeros
        SceneManager.LoadScene("Level1"); 
    }

    public void SalirJuego()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}