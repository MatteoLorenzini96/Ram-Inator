using System.Collections.Generic;
using UnityEngine;

public class LevelResultsManager : MonoBehaviour
{
    public static LevelResultsManager Instance; // Singleton per accesso globale

    [System.Serializable]
    public class LevelResult
    {
        public int levelIndex;
        public int stars;
    }

    public List<LevelResult> levelResults = new List<LevelResult>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveLevelResult(int levelIndex, int stars)
    {
        // Controlla se esiste già un risultato per questo livello
        var existingResult = levelResults.Find(result => result.levelIndex == levelIndex);
        if (existingResult != null)
        {
            existingResult.stars = Mathf.Max(existingResult.stars, stars); // Mantiene il massimo numero di stelle
        }
        else
        {
            levelResults.Add(new LevelResult { levelIndex = levelIndex, stars = stars });
        }
    }

    public int GetStarsForLevel(int levelIndex)
    {
        var result = levelResults.Find(r => r.levelIndex == levelIndex);
        return result != null ? result.stars : 0;
    }
}
