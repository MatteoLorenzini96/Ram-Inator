using UnityEngine;

public class ImpactHeadScript : MonoBehaviour
{
    [SerializeField] private Transform parentObject; // Il parent che contiene i child
    public int impactHeadIndex = 3; // L'indice del child da controllare
    public float lowImpactSpeed = 5f; // Soglia per impatti a bassa velocità
    public float highImpactSpeed = 10f; // Soglia per impatti ad alta velocità
    public GameObject objectToSpawn; // Prefab da spawnare al momento dell'impatto
    public float spawnDistance = -2f; // Distanza dal punto di impatto
    private Transform impactHead;
    private bool isScriptActive = false; // Stato attivazione script

    private void Start()
    {
        // Assicurati che il parentObject sia assegnato
        if (parentObject == null)
        {
            Debug.LogError("Parent Object non assegnato! Assegna un parent nel campo 'Parent Object' nell'Inspector.");
            return;
        }

        // Ottieni il child al determinato indice
        if (parentObject.childCount > impactHeadIndex)
        {
            impactHead = parentObject.GetChild(impactHeadIndex);
        }
        else
        {
            Debug.LogError($"Il parent non ha abbastanza child. Assicurati che ci sia un child all'indice {impactHeadIndex}.");
        }
    }

    private void Update()
    {
        if (impactHead != null)
        {
            bool isImpactHeadActive = impactHead.gameObject.activeSelf;
            bool isParentActive = impactHead.parent != null ? impactHead.parent.gameObject.activeSelf : false;

            // Log dello stato di ImpactHead e del suo parent
            Debug.Log($"ImpactHead ActiveSelf: {isImpactHeadActive}, Parent ActiveSelf: {isParentActive}");

            if (isImpactHeadActive && !isScriptActive)
            {
                isScriptActive = true;
                this.enabled = true; // Attiva lo script
                Debug.Log("Script attivato.");
            }
            else if (!isImpactHeadActive && isScriptActive)
            {
                isScriptActive = false;
                this.enabled = false; // Disattiva lo script
                Debug.Log("Script disattivato.");
            }
        }
        else
        {
            Debug.Log("ImpactHead non trovato o è null.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Esegui il codice solo se lo script è attivo
        if (isScriptActive)
        {
            // Calcola la velocità dell'impatto
            float relativeSpeed = collision.relativeVelocity.magnitude;

            // Controlla se l'oggetto colpito ha il tag "Metallo"
            if (collision.gameObject.CompareTag("Metallo"))
            {
                // Ottieni il punto di contatto
                ContactPoint contact = collision.contacts[0];
                Vector3 impactPoint = contact.point;
                Vector3 impactNormal = contact.normal;

                // Calcola il punto di spawn basandoti sull'angolazione e sulla distanza
                Vector3 spawnPoint = impactPoint + impactNormal * spawnDistance;

                // Calcola l'orientamento basato sulla normale e sulla velocità relativa dell'impatto
                Quaternion spawnRotation = Quaternion.LookRotation(collision.relativeVelocity.normalized, impactNormal);

                // Crea l'oggetto 3D al punto di impatto con offset
                if (objectToSpawn != null)
                {
                    GameObject spawnedObject = Instantiate(objectToSpawn, spawnPoint, spawnRotation);

                    // Modifica la scala dell'oggetto in base alla velocità
                    if (relativeSpeed <= lowImpactSpeed)
                    {
                        // Scala normale
                        spawnedObject.transform.localScale = Vector3.one;
                    }
                    else if (relativeSpeed >= highImpactSpeed)
                    {
                        // Scala aumentata di 4x
                        spawnedObject.transform.localScale = Vector3.one * 4f;
                    }
                    else
                    {
                        // Scala intermedia (calcolo lineare)
                        float scaleFactor = 1 + (relativeSpeed - lowImpactSpeed) / (highImpactSpeed - lowImpactSpeed) * 0.5f;
                        spawnedObject.transform.localScale = Vector3.one * scaleFactor;
                    }
                }
                else
                {
                    Debug.LogError("Nessun prefab assegnato a 'objectToSpawn'.");
                }
            }
        }
        else
        {
            Debug.Log("Lo script è disattivato, nessuna azione eseguita.");
        }
    }
}
