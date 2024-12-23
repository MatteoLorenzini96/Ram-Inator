using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public RawImage star1, star2, star3;
    public RawImage lvShow;

    public void Setup(int levelIndex)
    {
        // Aggiorna il numero di livello
        levelText.text = "Level " + levelIndex;

        // Ottieni il numero di stelle dal LevelResultsManager
        int stars = LevelResultsManager.Instance.GetStarsForLevel(levelIndex);

        // Aggiorna le stelle
        star1.gameObject.SetActive(stars >= 1);
        star2.gameObject.SetActive(stars >= 2);
        star3.gameObject.SetActive(stars >= 3);

        // Cambia colore se il livello è completato
        lvShow.color = stars > 0 ? Color.green : Color.white;
    }
}
