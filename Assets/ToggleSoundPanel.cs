using UnityEngine;

public class ToggleSoundPanel : MonoBehaviour
{
    // Riferimento all'oggetto UI SoundPanel
    public GameObject soundPanel;

    // Funzione per attivare/disattivare il SoundPanel
    public void TogglePanel()
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

        // Attiva o disattiva il pannello
        soundPanel.SetActive(!soundPanel.activeSelf);
    }
}
