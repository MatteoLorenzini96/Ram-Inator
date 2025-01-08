using UnityEngine;
using UnityEngine.UI;

public class CancelButton : MonoBehaviour
{
    public GameObject panelToClose; // La scheda (pannello UI) da chiudere
    public Button closeButton;      // Il bottone per chiudere la scheda

    void Start()
    {
        if (panelToClose == null)
        {
            Debug.LogError("PanelToClose non è assegnato!");
            return;
        }

        if (closeButton == null)
        {
            Debug.LogError("CloseButton non è assegnato!");
            return;
        }

        // Associa l'evento al bottone
        closeButton.onClick.AddListener(ClosePanel);
    }

    void ClosePanel()
    {
        panelToClose.SetActive(false); // Disattiva la scheda
        Time.timeScale = 1f;          // Riprende il tempo di gioco se era stato messo in pausa
    }

    void OnDestroy()
    {
        // Rimuovi il listener per evitare errori
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(ClosePanel);
        }
    }
}