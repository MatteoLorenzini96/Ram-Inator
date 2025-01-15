using System.Collections;
using UnityEngine;

public class ParentCollisionManager : MonoBehaviour
{
    public float forza = 10f; // La forza da applicare ai frammenti
    private float timeToReset = 1f; // Tempo in secondi prima di ripristinare il trigger
    private float timeToDestroy = 5f;
    private Vector3 impactPoint; // Punto di impatto ricevuto


    private void Awake()
    {
        // Applica la forza ai figli usando il punto di impatto
        foreach (Transform child in transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direzione = (child.position - impactPoint).normalized;
                rb.AddForce(direzione * forza, ForceMode.Impulse);
                //Debug.Log("Frammenti spinti nella direzione d'impatto");
            }
        }
    }
    
    // Metodo pubblico per impostare il punto di impatto
    public void SetImpactPoint(Vector3 point)
    {
        impactPoint = point;
        //Debug.Log("Punto d'impatto settato");
    }

    // Funzione pubblica che gestisce i trigger
    public void GestisciTrigger(Collider other, MonoBehaviour figlio)
    {
        Invoke("DestroyObject", timeToDestroy);

        /*
        //Debug.Log("Tag dell'oggetto che entra nel trigger: " + other.tag);

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
                Debug.Log("Impatto avvenuto con la Palla, spingo nella direzione d'impatto");
            }
        }
        */

        // Se l'oggetto che entra nel trigger è un "Muro"
        if (other.CompareTag("Metallo"))
        {
            Collider figlioCollider = figlio.GetComponent<Collider>();
            if (figlioCollider != null)
            {
                figlioCollider.isTrigger = false;
                //Debug.Log("Impatto avvenuto con il Metallo, ripristino l'impatto");

                // Avvia la coroutine per riabilitare il trigger dopo un certo tempo
                StartCoroutine(ResetTrigger(figlioCollider));
            }
        }

        /*
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
                    Debug.Log("Impatto avvenuto con il tag Fragment. Applico la forza nella direzione d'impatto della Palla");
                }
            }
        }
        */
    }

    // Aggiungi OnTriggerEnter per chiamare GestisciTrigger quando un oggetto entra nel trigger
    private void OnTriggerEnter(Collider other)
    {
        MonoBehaviour figlio = GetComponent<MonoBehaviour>(); // Ottieni il MonoBehaviour associato
        GestisciTrigger(other, figlio); // Chiamata corretta con due argomenti
    }

    // Coroutine per ripristinare il trigger dopo un certo tempo
    IEnumerator ResetTrigger(Collider collider)
    {
        //Debug.Log("Coroutine Attiva");

        // Aspetta per il tempo specificato
        yield return new WaitForSeconds(timeToReset);

        // Ripristina il trigger
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    void DestroyObject()
    {
        // Distruggere l'oggetto
        Destroy(gameObject);
    }

}
