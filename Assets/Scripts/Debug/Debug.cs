using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReset : MonoBehaviour
{
    private GameObject gearAndLevelHolder;
    private GameObject slider;

    [System.Obsolete]
    void Start()
    {
        // Cerca gli oggetti nella scena, inclusi quelli disattivati
        var allTransforms = FindObjectsOfType<Transform>(true);

        foreach (var t in allTransforms)
        {
            if (t.name == "GearAndLevelHolder")
                gearAndLevelHolder = t.gameObject;
            else if (t.name == "Slider")
                slider = t.gameObject;
        }

        if (gearAndLevelHolder == null || slider == null)
        {
            Debug.LogError("Impossibile trovare GearAndLevelHolder o Slider. Assicurati che esistano nella scena.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Debug.Log("Resetto la scena");

            // Ricarica la scena attuale
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            NextScene();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            //Debug.Log("Cambio UI");

            // Alterna tra GearAndLevelHolder e Slider
            if (gearAndLevelHolder != null && slider != null)
            {
                bool isGearActive = gearAndLevelHolder.activeSelf;

                // Imposta lo stato attivo/inattivo alternato
                gearAndLevelHolder.SetActive(!isGearActive);
                slider.SetActive(isGearActive);

                //Debug.Log($"GearAndLevelHolder attivo: {!isGearActive}, Slider attivo: {isGearActive}");
            }
        }
    }

    private void NextScene()
    {
        // Ottieni l'indice della scena attuale
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // Calcola l'indice della prossima scena
        int nextSceneIndex = currentSceneIndex + 1;

        // Controlla se l'indice della prossima scena è valido
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Carica la scena successiva
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Non ci sono altre scene da caricare!");
        }
    }
}
