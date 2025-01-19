using UnityEngine;

public class MaterialOpacityController : MonoBehaviour
{
    private WreckingBallDrag wreckingBallDrag;
    private SlideAncor slideAncor;
    private Material objectMaterial;
    private Color originalEmissionColor;
    private Color draggingEmissionColor = new Color(70f / 255f, 70f / 255f, 70f / 255f); // Convertito in scala 0–1

    void Start()
    {
        slideAncor = FindFirstObjectByType<SlideAncor>();
        if (slideAncor == null)
        {
            Debug.LogError("SlideAncor non trovato");
            return;
        }

        wreckingBallDrag = FindFirstObjectByType<WreckingBallDrag>();
        if (wreckingBallDrag == null)
        {
            Debug.LogError("WreckingBallDrag non trovato");
            return;
        }

        // Iscriviti all'evento OnResetPosition
        wreckingBallDrag.OnResetPosition += HandleResetPosition;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            objectMaterial = renderer.material;

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

        if (wreckingBallDrag.isDragging || slideAncor.anchorMoving)
        {
            ChangeColor();
        }
        else
        {
            ResetColor();
        }
    }

    void ChangeColor()
    {
        objectMaterial.SetColor("_EmissionColor", draggingEmissionColor);
    }

    public void ResetColor()
    {
        objectMaterial.SetColor("_EmissionColor", originalEmissionColor);
    }

    private void HandleResetPosition()
    {
        // Azione da eseguire quando viene chiamata ResetPosition
        //Debug.Log("ResetPosition chiamato - Ripristino il colore.");
        ResetColor();
    }

    private void OnDestroy()
    {
        // Rimuovi l'iscrizione all'evento per evitare memory leak
        if (wreckingBallDrag != null)
        {
            wreckingBallDrag.OnResetPosition -= HandleResetPosition;
        }
    }
}
