using UnityEngine;

public class HeightController : MonoBehaviour
{
    public Transform pivot; // Oggetto pivot (genitore dei punti di attacco)
    public Transform ariete; // Oggetto ariete
    public Transform pivotPoint1, pivotPoint2; // Punti sul pivot
    public Transform arietePoint1, arietePoint2; // Punti sull'ariete
    private LineRenderer lineRenderer;

    void Start()
    {
        // Ottieni il LineRenderer dall'oggetto corrente
        lineRenderer = GetComponent<LineRenderer>();

        // Configura il LineRenderer per 4 punti (due linee)
        lineRenderer.positionCount = 4;
    }

    void Update()
    {
        // Modifica la posizione del pivot per cambiare la lunghezza
        if (Input.GetKey(KeyCode.UpArrow))
        {
            pivot.position += Vector3.up * Time.deltaTime;
            Debug.Log("Accorcia il pendolo");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            pivot.position -= Vector3.up * Time.deltaTime;
            Debug.Log("Allunga il pendolo");
        }

        // Aggiorna i punti del LineRenderer
        UpdateLineRenderer();
    }

    void UpdateLineRenderer()
    {
        // Imposta i punti per disegnare le due linee
        lineRenderer.SetPosition(0, pivotPoint1.position); // Primo punto sulla coppia del pivot
        lineRenderer.SetPosition(1, arietePoint1.position); // Primo punto sulla coppia dell'ariete

        lineRenderer.SetPosition(2, arietePoint2.position); // Secondo punto sulla coppia dell'ariete
        lineRenderer.SetPosition(3, pivotPoint2.position); // Secondo punto sulla coppia del pivot
    }
}
