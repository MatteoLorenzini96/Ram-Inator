using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Per la gestione delle scene

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public RawImage star1, star2, star3;
    public RawImage lvShow;

    private int levelIndex; // Salva l'indice del livello associato a questo pulsante

    public void Setup(int levelIndex)
    {
        this.levelIndex = levelIndex; // Salva l'indice del livello per usi successivi

        // Aggiorna il numero di livello, ignorando i primi due (menu e CentralHub)
        int displayedLevelNumber = levelIndex - 1;
        if (displayedLevelNumber > 0) // Assicurati di mostrare solo per livelli validi
        {
            levelText.text = "Level " + displayedLevelNumber;
        }
        else
        {
            levelText.text = "Invalid Level";
            Debug.LogWarning($"Livello con indice {levelIndex} non valido per il display!");
        }

        // Ottieni il numero di stelle dal LevelResultsManager
        int stars = LevelResultsManager.Instance.GetStarsForLevel(levelIndex);

        // Aggiorna le stelle
        star1.gameObject.SetActive(stars >= 1);
        star2.gameObject.SetActive(stars >= 2);
        star3.gameObject.SetActive(stars >= 3);

        // Cambia colore se il livello è completato
        lvShow.color = stars > 0 ? Color.green : Color.white;
    }

    public void LoadLevel()
    {
        if (levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Caricamento della scena: {levelIndex}");
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogError($"L'indice del livello {levelIndex} è fuori dai limiti del Build Settings!");
        }
    }
}
