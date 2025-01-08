using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneButton : MonoBehaviour
{
    // Metodo per ricaricare la scena corrente
    public void ReloadScene()
    {
        // Ottiene il nome della scena corrente e la ricarica
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}