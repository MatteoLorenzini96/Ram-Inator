using UnityEngine;

public class MaterialOpacityController : MonoBehaviour
{
    // Riferimento al componente WreckingBallDrag
    private WreckingBallDrag wreckingBallDrag;
    private SlideAncor slideAncor;

    // Materiale originale e colore iniziale
    private Material objectMaterial;
    private Color originalEmissionColor;

    // Colore di emissione desiderato durante il drag
    private Color draggingEmissionColor = new Color(70f / 255f, 70f / 255f, 70f / 255f); // Convertito in scala 0–1

    void Start()
    {
        // Cerca automaticamente lo script SlideAncor
        slideAncor = FindFirstObjectByType<SlideAncor>();
        if (slideAncor == null)
        {
            Debug.LogError("SlideAncor non trovato");
            return;
        }

        // Cerca automaticamente lo script WreckingBallDrag
        wreckingBallDrag = FindFirstObjectByType<WreckingBallDrag>();
        if (wreckingBallDrag == null)
        {
            Debug.LogError("WreckingBallDrag non trovato");
            return;
        }

        // Ottieni il materiale dell'oggetto
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            objectMaterial = renderer.material;

            // Controlla che il materiale supporti emissione
            if (objectMaterial.HasProperty("_EmissionColor"))
            {
                originalEmissionColor = objectMaterial.GetColor("_EmissionColor");
            }
            else
            {
                Debug.LogError("Il materiale non supporta la proprietà _EmissionColor.");
            }
        }
        else
        {
            Debug.LogError("Renderer non trovato su " + gameObject.name);
        }
    }

    void Update()
    {
        if (wreckingBallDrag == null || objectMaterial == null || slideAncor == null) return;

        // Controlla lo stato di isDragging e slideAncor e modifica l'emissione
        if (wreckingBallDrag.isDragging || slideAncor.anchorMoving)
        {
            // Imposta il colore di emissione desiderato
            objectMaterial.SetColor("_EmissionColor", draggingEmissionColor);

            // Attiva l'emissione nel materiale (se non è già attiva)
            objectMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            // Ripristina il colore di emissione originale
            objectMaterial.SetColor("_EmissionColor", originalEmissionColor);

            // Attiva l'emissione per garantire che il materiale torni allo stato originale
            objectMaterial.EnableKeyword("_EMISSION");
        }
    }
}
