using UnityEngine;

public class TrailSizeAndColorController : MonoBehaviour
{
    private TrailRenderer trailRenderer; // Trail Renderer dell'oggetto
    private Rigidbody rb; // Rigidbody dell'oggetto
    private WreckingBallDrag wreckingBallDrag;

    public float maxWidth = 0.5f; // Larghezza massima del trail
    public float minWidth = 0.13f; // Larghezza minima del trail
    public float speedFactor = 2f; // Fattore di scala per la velocità

    [System.Serializable]
    public class ChildColorPreset
    {
        public string childName; // Nome del figlio
        public Gradient colorGradient; // Gradiente associato al figlio
    }

    public ChildColorPreset[] childColorPresets; // Preset di colori per i figli

    void Start()
    {
        // Ottieni il Trail Renderer collegato all'oggetto
        trailRenderer = GetComponent<TrailRenderer>();

        // Ottieni il Rigidbody dell'oggetto
        rb = GetComponent<Rigidbody>();

        // Ottieni il WreckingBallDrag
        wreckingBallDrag = GetComponent<WreckingBallDrag>();

        // Assicurati che il Rigidbody sia presente
        if (rb == null)
        {
            Debug.LogError("Rigidbody non trovato! Assicurati che l'oggetto abbia un componente Rigidbody.");
        }
    }

    void Update()
    {
        if (rb != null)
        {
            if (wreckingBallDrag != null && wreckingBallDrag.isSwinging)
            {
                // Aggiorna la larghezza del trail
                UpdateTrailWidth();
            }
            else
            {
                // Se non sta oscillando, nascondi il trail
                trailRenderer.widthMultiplier = 0f;
            }

            // Aggiorna sempre il colore del trail in base al figlio attivo
            UpdateTrailColor();
        }
    }

    void UpdateTrailWidth()
    {
        // Ottieni la velocità dell'oggetto dal Rigidbody
        float objectSpeed = rb.linearVelocity.magnitude; // Usa velocity.magnitude per la velocità scalare

        // Calcola la larghezza del trail basata sulla velocità
        float trailWidth = Mathf.Clamp(objectSpeed * speedFactor, minWidth, maxWidth);

        // Aggiorna la larghezza del Trail Renderer
        trailRenderer.widthMultiplier = trailWidth;
    }

    void UpdateTrailColor()
    {
        // Controlla ogni figlio dell'oggetto
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                // Trova il preset corrispondente al nome del figlio attivo
                foreach (var preset in childColorPresets)
                {
                    if (child.name == preset.childName)
                    {
                        // Cambia il gradiente del Trail Renderer
                        trailRenderer.colorGradient = preset.colorGradient;
                        return;
                    }
                }

                Debug.LogWarning($"Nessun preset trovato per il figlio attivo: {child.name}");
            }
        }
    }
}
