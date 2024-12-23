using UnityEngine;

public class LevelButtonSpawner : MonoBehaviour
{
    public GameObject levelButtonPrefab; // Prefab da assegnare nell'Inspector

    private void Start()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

        for (int i = 2; i < sceneCount; i++) // Salta le prime 2 scene
        {
            GameObject newButton = Instantiate(levelButtonPrefab, transform);
            LevelButton levelButton = newButton.GetComponent<LevelButton>();
            if (levelButton != null)
            {
                levelButton.Setup(i);
            }
        }
    }
}
