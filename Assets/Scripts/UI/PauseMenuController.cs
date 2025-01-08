using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI; // Riferimento al pannello del menù di pausa
    public Button pauseButton;     // Riferimento al bottone per attivare/disattivare il menù di pausa

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI == null)
        {
            Debug.LogError("PauseMenuUI non è assegnato!");
            return;
        }

        if (pauseButton == null)
        {
            Debug.LogError("PauseButton non è assegnato!");
            return;
        }

        // Associa l'evento del bottone
        pauseButton.onClick.AddListener(TogglePauseMenu);

        // Assicura che il menù di pausa sia inizialmente disattivato
        pauseMenuUI.SetActive(false);
    }

    void TogglePauseMenu()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Ferma il tempo di gioco
    }

    void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Ripristina il tempo di gioco
    }

    void OnDestroy()
    {
        // Rimuovi il listener per evitare errori
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveListener(TogglePauseMenu);
        }
    }
}