using UnityEngine;

public class SpikeCollision : MonoBehaviour
{
    // Nome del figlio specifico che attiva il trigger
    private string spikeHeadName = "SpikeHead";

    // Nome del genitore che contiene i figli da monitorare
    private string parentObjectName = "WrekingBallHolder";

    // Riferimento al Collider dell'oggetto
    private Collider objectCollider;

    // Riferimento al CollisionStateChanger per manipolare direttamente il suo stato
    private CollisionStateChanger stateChanger;

    // Riferimento al genitore dei figli
    private Transform parentTransform;

    private void Awake()
    {
        // Ottieni il Collider dell'oggetto
        objectCollider = GetComponent<Collider>();

        // Trova lo script CollisionStateChanger attaccato allo stesso oggetto
        stateChanger = GetComponent<CollisionStateChanger>();

        if (stateChanger == null)
        {
            Debug.LogError("Nessun componente CollisionStateChanger trovato su questo oggetto!");
        }

        // Trova il genitore specifico
        GameObject parentObject = GameObject.Find(parentObjectName);
        if (parentObject != null)
        {
            parentTransform = parentObject.transform;
        }
        else
        {
            Debug.LogError($"Oggetto genitore '{parentObjectName}' non trovato nella scena!");
        }
    }

    private void Update()
    {
        // Controlla il figlio attivo nel genitore
        if (parentTransform != null)
        {
            bool isSpikeHeadActive = false;

            foreach (Transform child in parentTransform)
            {
                if (child.gameObject.activeSelf && child.name == spikeHeadName)
                {
                    isSpikeHeadActive = true;
                    break;
                }
            }

            // Imposta isTrigger in base al risultato
            if (objectCollider != null)
            {
                objectCollider.isTrigger = isSpikeHeadActive;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il trigger viene attivato da un oggetto con il tag "Palla"
        if (other.CompareTag("Palla"))
        {
            // Porta le vite dell'oggetto a 0
            if (stateChanger != null)
            {
                stateChanger.viteoggetto = 0;

                // Attiva l'esplosione
                stateChanger.Explode();
            }
        }
    }
}
