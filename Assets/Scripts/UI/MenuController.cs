using UnityEngine;
using UnityEngine.SceneManagement; // Necessario per gestire le scene

public class MenuController : MonoBehaviour
{
    // Metodo per caricare la scena successiva
    public void NextScene()
    {
        // Ottieni l'indice della scena attuale
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // Calcola l'indice della prossima scena
        int nextSceneIndex = currentSceneIndex + 1;

        // Controlla se l'indice della prossima scena è valido
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Carica la scena successiva
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Non ci sono altre scene da caricare!");
        }
    }

    // Metodo per chiudere il gioco
    public void Quit()
    {
        // Messaggio utile durante lo sviluppo
        Debug.Log("Uscita dal gioco...");

        // Chiude l'applicazione
        Application.Quit();
    }
}
