using UnityEngine;

public class MaterialOpacityController : MonoBehaviour
{
    // Campo per impostare l'opacità desiderata dall'inspector
    [Range(0f, 1f)] public float opacityWhenDragging = 0.5f;

    // Riferimento al componente WreckingBallDrag
    private WreckingBallDrag wreckingBallDrag;
    private SlideAncor slideAncor;

    // Materiale originale e colore iniziale
    private Material objectMaterial;
    private Color originalColor;

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
            originalColor = objectMaterial.color;
        }
        else
        {
            Debug.LogError("Renderer non trovato su " + gameObject.name);
        }
    }

    void Update()
    {
        if (wreckingBallDrag == null || objectMaterial == null || slideAncor ==null) return;

        // Controlla lo stato di isDragging e slideAncor e modifica il colore
        if (wreckingBallDrag.isDragging || slideAncor.anchorMoving == true)
        {
            // Imposta il colore con l'opacità ridotta
            objectMaterial.color = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f, opacityWhenDragging);
        }
        else
        {
            // Ripristina il colore originale
            objectMaterial.color = originalColor;
        }
    }
}
