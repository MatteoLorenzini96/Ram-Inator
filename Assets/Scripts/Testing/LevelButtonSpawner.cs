using UnityEngine;
using UnityEngine.UI;

public class LevelButtonSpawner : MonoBehaviour
{
    public GameObject levelButtonPrefab; // Prefab da assegnare nell'Inspector
    public Button nextButton; // Bottone "Avanti"
    public Button prevButton; // Bottone "Indietro"

    private const int ButtonsPerPage = 4; // Numero massimo di pulsanti per pagina
    private int currentPage = 0; // Pagina attuale
    private int totalPages; // Numero totale di pagine
    private GameObject[] levelButtons; // Array per salvare i pulsanti creati

    private void Start()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

        // Crea i pulsanti
        levelButtons = new GameObject[sceneCount - 2];
        for (int i = 2; i < sceneCount; i++)
        {
            GameObject newButton = Instantiate(levelButtonPrefab, transform);
            LevelButton levelButton = newButton.GetComponent<LevelButton>();
            if (levelButton != null)
            {
                levelButton.Setup(i);
            }
            newButton.SetActive(false); // Nascondi tutti i pulsanti inizialmente
            levelButtons[i - 2] = newButton;
        }

        // Calcola il numero totale di pagine
        totalPages = Mathf.CeilToInt((float)levelButtons.Length / ButtonsPerPage);

        // Configura i pulsanti freccia
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PreviousPage);

        UpdatePage();
    }

    private void UpdatePage()
    {
        // Disattiva tutti i pulsanti
        foreach (GameObject button in levelButtons)
        {
            button.SetActive(false);
        }

        // Attiva solo i pulsanti della pagina corrente
        int startIndex = currentPage * ButtonsPerPage;
        int endIndex = Mathf.Min(startIndex + ButtonsPerPage, levelButtons.Length);

        for (int i = startIndex; i < endIndex; i++)
        {
            levelButtons[i].SetActive(true);
        }

        // Aggiorna lo stato dei pulsanti "Avanti" e "Indietro"
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < totalPages - 1;
    }

    private void NextPage()
    {
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            UpdatePage();
        }
    }

    private void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }
}