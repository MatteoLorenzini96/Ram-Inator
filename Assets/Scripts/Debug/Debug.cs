using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReset : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Resetto la scena");

            // Ricarica la scena attuale
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
