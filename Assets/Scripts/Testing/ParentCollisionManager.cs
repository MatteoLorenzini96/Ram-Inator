using UnityEngine;

public class ParentCollisionManager : MonoBehaviour
{
    public float forza = 10f; // La forza da applicare ai frammenti

    // Funzione pubblica che gestisce i trigger
    public void GestisciTrigger(Collider other, MonoBehaviour figlio)
    {
        // Se l'oggetto che entra nel trigger è la "Palla"
        if (other.CompareTag("Palla"))
        {
            // Calcoliamo il punto di impatto della Palla (utilizzando il punto più vicino)
            Vector3 puntoDiImpatto = other.ClosestPointOnBounds(transform.position);

            // Recupera il Rigidbody del frammento (figlio)
            Rigidbody rb = figlio.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calcola la direzione dal punto di impatto della Palla al centro del frammento
                Vector3 direzione = (transform.position - puntoDiImpatto).normalized;

                // Applica la forza nella direzione calcolata
                rb.AddForce(direzione * forza, ForceMode.Impulse);
            }
        }
        // Se l'oggetto che entra nel trigger è un "Fragment"
        else if (other.CompareTag("Fragment"))
        {
            // Cerchiamo la Palla nel mondo
            GameObject palla = GameObject.FindGameObjectWithTag("Palla");

            if (palla != null)
            {
                // Calcoliamo il punto di impatto della Palla rispetto al Fragment
                Vector3 puntoDiImpatto = other.ClosestPointOnBounds(palla.transform.position);

                // Recupera il Rigidbody del frammento (figlio)
                Rigidbody rb = figlio.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Calcola la direzione dal punto di impatto della Palla al centro del frammento
                    Vector3 direzione = (transform.position - puntoDiImpatto).normalized;

                    // Applica la forza nella direzione calcolata
                    rb.AddForce(direzione * forza, ForceMode.Impulse);
                }
            }
        }
    }

    // Aggiungi OnTriggerEnter per chiamare GestisciTrigger quando un oggetto entra nel trigger
    private void OnTriggerEnter(Collider other)
    {
        MonoBehaviour figlio = GetComponent<MonoBehaviour>(); // Ottieni il MonoBehaviour associato
        GestisciTrigger(other, figlio); // Chiamata corretta con due argomenti
    }
}
