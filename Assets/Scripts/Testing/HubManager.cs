using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HubManager : MonoBehaviour
{
    public TextMeshProUGUI progressText;

    private void Start()
    {
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        string progress = "Progressi:\n";
        for (int i = 1; i <= GameManager.Instance.totalLevels; i++)
        {
            int stars = GameManager.Instance.GetLevelStars(i);
            progress += $"Livello {i}: {stars} stelle\n";
        }

        progressText.text = progress;
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
}
