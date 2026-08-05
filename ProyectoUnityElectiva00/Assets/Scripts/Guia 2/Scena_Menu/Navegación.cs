using UnityEngine;
using UnityEngine.SceneManagement;

public class Navegación : MonoBehaviour
{
    // Carga la escena por el nombre exacto que tiene en Unity
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Carga la escena por su número de índice en Build Settings (0, 1, 2...)
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Cierra la aplicación (funciona en el celular / APK)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo de la aplicación...");
    }
}