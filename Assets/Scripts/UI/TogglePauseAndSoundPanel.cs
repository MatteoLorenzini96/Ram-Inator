using UnityEngine;

public class TogglePauseAndSoundPanel : MonoBehaviour
{
    // Riferimenti agli oggetti UI
    public GameObject pausePanel;
    public GameObject soundPanel;

    // Funzione per attivare/disattivare i pannelli
    public void TogglePanels()
    {
        // Verifica se il "PausePanel" è aperto
        if (pausePanel.activeSelf)
        {
            // Disattiva il "PausePanel"
            pausePanel.SetActive(false);
        }

        // Attiva il "SoundPanel"
        soundPanel.SetActive(!soundPanel.activeSelf);
    }
}