using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalLevels = 5; // Numero totale di livelli
    public Dictionary<int, int> levelStars = new Dictionary<int, int>(); // Livelli completati e numero di stelle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantiene l'oggetto quando si cambia scena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLevelStars(int level, int stars)
    {
        if (levelStars.ContainsKey(level))
        {
            // Aggiorna le stelle solo se il nuovo punteggio è migliore
            if (levelStars[level] < stars)
                levelStars[level] = stars;
        }
        else
        {
            levelStars.Add(level, stars);
        }
    }

    public int GetLevelStars(int level)
    {
        return levelStars.ContainsKey(level) ? levelStars[level] : 0;
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SaveProgress(levelStars);
    }

    private void Start()
    {
        levelStars = SaveSystem.LoadProgress();
    }

}
