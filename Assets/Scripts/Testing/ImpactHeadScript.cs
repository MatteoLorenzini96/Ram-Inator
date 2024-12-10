using UnityEngine;

public class WreckingBall : MonoBehaviour
{
    [SerializeField] private Transform parentObject; // Il parent che contiene i child
    public int impactHeadIndex = 3; // L'indice del child da controllare
    public Vector3 boxSize = new Vector3(1, 1, 1); // Dimensione del box collider generato
    private Transform impactHead;

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

    private void OnCollisionEnter(Collision collision)
    {
        // Controlla se l'oggetto ImpactHead è attivo
        if (impactHead != null && impactHead.gameObject.activeSelf)
        {
            // Controlla se l'oggetto colpito ha il tag "Metallo"
            if (collision.gameObject.CompareTag("Metallo"))
            {
                // Ottieni il punto di contatto
                ContactPoint contact = collision.contacts[0];
                Vector3 impactPoint = contact.point;
                Vector3 impactNormal = contact.normal;

                // Crea un nuovo oggetto con un BoxCollider
                GameObject boxColliderObject = new GameObject("GeneratedBoxCollider");
                boxColliderObject.transform.position = impactPoint;
                boxColliderObject.transform.rotation = Quaternion.LookRotation(impactNormal);

                // Aggiungi un BoxCollider al nuovo oggetto
                BoxCollider boxCollider = boxColliderObject.AddComponent<BoxCollider>();
                boxCollider.size = boxSize;

                // (Opzionale) Aggiungi un Rigidbody per stabilità
                boxColliderObject.AddComponent<Rigidbody>().isKinematic = true;

                Debug.Log("Box Collider creato al punto di impatto!");
            }
        }
        else
        {
            Debug.Log("Il child specificato non è attivo. Nessuna azione eseguita.");
        }
    }
}
