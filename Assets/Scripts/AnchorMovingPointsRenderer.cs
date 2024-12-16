using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AnchorMovingPointsRenderer : MonoBehaviour
{
    // Riferimenti ai punti impostabili dall'Inspector
    public Transform PuntoA;
    public Transform PuntoB;

    // Riferimento allo script "SlideAncor" del padre
    private SlideAncor slideAncorScript;

    // LineRenderer
    private LineRenderer lineRenderer;

    void Start()
    {
        // Ottiene il LineRenderer dall'oggetto
        lineRenderer = GetComponent<LineRenderer>();

        // Controlla se i punti sono stati assegnati
        if (PuntoA == null || PuntoB == null)
        {
            Debug.LogError("PuntoA o PuntoB non assegnati nel LineRendererController!");
            return;
        }

        // Inizializza il LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, PuntoA.position);
        lineRenderer.SetPosition(1, PuntoB.position);

        // Trova lo script SlideAncor nel padre
        slideAncorScript = GetComponentInParent<SlideAncor>();
        if (slideAncorScript == null)
        {
            Debug.LogError("Script 'SlideAncor' non trovato nel genitore di " + gameObject.name);
        }
        else
        {
            // Imposta lo stato iniziale del LineRenderer in base allo stato di SlideAncor
            lineRenderer.enabled = slideAncorScript.isActiveAndEnabled;
        }
    }

    void Update()
    {
        // Controlla se lo script SlideAncor è attivo
        if (slideAncorScript != null)
        {
            lineRenderer.enabled = slideAncorScript.isActiveAndEnabled;
        }
    }
}
