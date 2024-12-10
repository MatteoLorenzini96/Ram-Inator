using UnityEngine;

public class DragObject : MonoBehaviour
{
    // Limiti per il movimento lungo l'asse X
    public GameObject puntoA;
    public GameObject puntoB;

    // Variabili per la gestione del trascinamento
    private bool isDragging = false;
    private float offsetX;

    void Update()
    {
        // Controlla se il mouse è premuto
        if (Input.GetMouseButtonDown(0)) // 0 = Click sinistro del mouse
        {
            // Verifica se il mouse è sopra l'oggetto
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Se l'oggetto è stato colpito, inizia il trascinamento
                if (hit.transform == transform)
                {
                    isDragging = true;
                    offsetX = hit.point.x - transform.position.x; // Calcola l'offset tra il mouse e l'oggetto

                    Debug.Log("Oggetto Preso");
                }
            }
        }

        // Se il mouse è tenuto premuto, trascina l'oggetto
        if (isDragging)
        {
            // Ottieni la posizione del mouse nel mondo
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Calcola la nuova posizione lungo l'asse X, mantenendo gli altri assi invariati
                float newX = hit.point.x - offsetX;

                // Limita la posizione X tra i valori min e max
                newX = Mathf.Clamp(newX, puntoA.transform.position.x, puntoB.transform.position.x);

                // Imposta la nuova posizione dell'oggetto
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            }
        }

        // Se il mouse viene rilasciato, termina il trascinamento
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
}
