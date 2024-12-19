using UnityEngine;
using UnityEngine.SceneManagement; // Necessario per gestire le scene

public class MenuController : MonoBehaviour
{

    public GameObject soundPanel;
    public GameObject backgroundPanel;

    public void TogglePlayPanel()
    {
        // Se il riferimento non è assegnato, cerca automaticamente
        if (backgroundPanel == null)
        {
            backgroundPanel = GameObject.Find("BackgroundPanel");
            if (backgroundPanel == null)
            {
                Debug.LogError("BackgroundPanel non trovato nella scena. Assicurati che l'oggetto sia chiamato 'BackgroundPanel' o assegnalo manualmente.");
                return;
            }
        }

        backgroundPanel.SetActive(true);
        soundPanel.SetActive(false);

    }

    public void ToggleSoundPanel()
    {
        // Se il riferimento non è assegnato, cerca automaticamente
        if (soundPanel == null)
        {
            soundPanel = GameObject.Find("SoundPanel");
            if (soundPanel == null)
            {
                Debug.LogError("SoundPanel non trovato nella scena. Assicurati che l'oggetto sia chiamato 'SoundPanel' o assegnalo manualmente.");
                return;
            }
        }

        backgroundPanel.SetActive(false);
        soundPanel.SetActive(true);
    }


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
