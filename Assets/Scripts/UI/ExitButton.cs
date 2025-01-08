using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    public Button exitButton; // Il bottone per uscire dal gioco

    void Start()
    {
        if (exitButton == null)
        {
            Debug.LogError("ExitButton non è assegnato!");
            return;
        }

        // Associa l'evento del bottone
        exitButton.onClick.AddListener(ExitGame);
    }

    void ExitGame()
    {
        Debug.Log("Uscita dal gioco...");

        // Chiude l'applicazione
        Application.Quit();

#if UNITY_EDITOR
        // Questo funziona solo nell'Editor di Unity per simulare l'uscita
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void OnDestroy()
    {
        // Rimuovi il listener per evitare errori
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(ExitGame);
        }
    }
}