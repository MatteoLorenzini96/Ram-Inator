using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    private static string saveKey = "GameSave";

    public static void SaveProgress(Dictionary<int, int> levelStars)
    {
        string json = JsonUtility.ToJson(new SaveData(levelStars));
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
    }

    public static Dictionary<int, int> LoadProgress()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string json = PlayerPrefs.GetString(saveKey);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data.levelStars;
        }
        return new Dictionary<int, int>();
    }

    [System.Serializable]
    private class SaveData
    {
        public Dictionary<int, int> levelStars;

        public SaveData(Dictionary<int, int> stars)
        {
            levelStars = stars;
        }
    }
}
