using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Per la gestione delle scene

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Image star1, star2, star3;
    public Image lvShow;

    private int levelIndex; // Salva l'indice del livello associato a questo pulsante

    public void Setup(int levelIndex)
    {
        this.levelIndex = levelIndex; // Salva l'indice del livello per usi successivi

        // Aggiorna il numero di livello, ignorando i primi due (menu e CentralHub)
        int displayedLevelNumber = levelIndex - 1;
        if (displayedLevelNumber > 0) // Assicurati di mostrare solo per livelli validi
        {
            levelText.text = "" + displayedLevelNumber;
        }
        else
        {
            levelText.text = "Invalid Level";
            Debug.LogWarning($"Livello con indice {levelIndex} non valido per il display!");
        }

        // Ottieni il numero di stelle dal LevelResultsManager
        int stars = LevelResultsManager.Instance.GetStarsForLevel(levelIndex);

        // Aggiorna le stelle
        star1.color = stars >= 1 ? Color.white : Color.grey;
        star2.color = stars >= 2 ? Color.white : Color.grey;
        star3.color = stars >= 3 ? Color.white : Color.grey;

        /*star1.gameObject.SetActive(stars >= 1);
        star2.gameObject.SetActive(stars >= 2);
        star3.gameObject.SetActive(stars >= 3);*/

        // Cambia colore se il livello è completato
        lvShow.color = stars > 0 ? Color.white : Color.grey;
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
