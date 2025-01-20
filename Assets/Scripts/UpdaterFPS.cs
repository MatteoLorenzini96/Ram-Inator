using TMPro;
using UnityEngine;

public class UpdaterFPS : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private float deltaTime = 0.0f;

    void Awake()
    {
        // Ottieni il componente TextMeshProUGUI
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("Il componente TextMeshProUGUI non è stato trovato! Assicurati di averlo aggiunto al GameObject.");
        }
    }

    void Update()
    {
        // Calcola il deltaTime per gli FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        // Aggiorna il testo di TextMeshPro
        if (textMesh != null)
        {
            textMesh.text = $"FPS: {Mathf.Ceil(fps)}";
        }
    }
}
