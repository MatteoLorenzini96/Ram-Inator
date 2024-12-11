using UnityEngine;

public class WreckingBall : MonoBehaviour
{
    [SerializeField] private Transform parentObject; // Il parent che contiene i child
    public int impactHeadIndex = 3; // L'indice del child da controllare
    public float lowImpactSpeed = 5f; // Soglia per impatti a bassa velocità
    public float highImpactSpeed = 10f; // Soglia per impatti ad alta velocità
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
            // Calcola la velocità dell'impatto
            float relativeSpeed = collision.relativeVelocity.magnitude;

            // Controlla se l'oggetto colpito ha il tag "Metallo"
            if (collision.gameObject.CompareTag("Metallo"))
            {
                // Ottieni il punto di contatto
                ContactPoint contact = collision.contacts[0];
                Vector3 impactPoint = contact.point;
                Vector3 impactNormal = contact.normal;

                // Sposta il punto di impatto di 1 unità lungo l'asse X
                Vector3 adjustedImpactPoint = impactPoint + (impactNormal * 1f); // Aggiungi offset di 1 lungo l'asse X

                // Crea un nuovo oggetto con un CapsuleCollider (come un conico)
                GameObject colliderObject = new GameObject("GeneratedCollider");
                colliderObject.transform.position = adjustedImpactPoint;
                colliderObject.transform.rotation = Quaternion.LookRotation(impactNormal);

                // Aggiungi un CapsuleCollider (conico)
                CapsuleCollider capsuleCollider = colliderObject.AddComponent<CapsuleCollider>();

                // Imposta le dimensioni del collider in base alla velocità
                if (relativeSpeed <= lowImpactSpeed)
                {
                    capsuleCollider.height = 1f; // Piccolo collider
                    capsuleCollider.radius = 0.5f;
                }
                else if (relativeSpeed >= highImpactSpeed)
                {
                    capsuleCollider.height = 3f; // Grande collider
                    capsuleCollider.radius = 1f;
                }
                else
                {
                    capsuleCollider.height = 2f; // Dimensione media
                    capsuleCollider.radius = 0.75f;
                }

                // (Opzionale) Aggiungi un Rigidbody per stabilità
                capsuleCollider.gameObject.AddComponent<Rigidbody>().isKinematic = true;

                // Imposta viteoggetto a 0 per distruggere l'oggetto
                CollisionStateChanger collisionStateChanger = collision.gameObject.GetComponent<CollisionStateChanger>();
                if (collisionStateChanger != null)
                {
                    collisionStateChanger.viteoggetto = 0;
                    Debug.Log("Viteoggetto settato a 0.");
                }

                Debug.Log("Collider conico creato al punto di impatto con offset!");
            }
        }
        else
        {
            Debug.Log("Il child specificato non è attivo. Nessuna azione eseguita.");
        }
    }

    // Visualizza i Gizmos per il Collider
    private void OnDrawGizmos()
    {
        if (impactHead != null && impactHead.gameObject.activeSelf)
        {
            // Simula il punto di impatto e applica l'offset di 1 unità lungo l'asse X
            Vector3 adjustedImpactPoint = impactHead.position + Vector3.right; // Aggiungi l'offset sull'asse X

            // Imposta una velocità di esempio per il collider
            float relativeSpeed = 7f;

            // Determina la dimensione del collider in base alla velocità
            float colliderHeight = 2f;
            float colliderRadius = 0.75f;

            if (relativeSpeed <= lowImpactSpeed)
            {
                colliderHeight = 1f; // Piccolo collider
                colliderRadius = 0.5f;
            }
            else if (relativeSpeed >= highImpactSpeed)
            {
                colliderHeight = 3f; // Grande collider
                colliderRadius = 1f;
            }

            // Disegna il collider con Gizmos
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(adjustedImpactPoint, colliderRadius); // Mostra una sfera per visualizzare la base
            Gizmos.DrawLine(adjustedImpactPoint, adjustedImpactPoint + (Vector3.up * colliderHeight)); // Mostra l'asse del collider

            // Disegna la capsula/cono
            Gizmos.DrawLine(adjustedImpactPoint, adjustedImpactPoint + (Vector3.right * colliderRadius)); // Linea orizzontale
            Gizmos.DrawLine(adjustedImpactPoint + (Vector3.up * colliderHeight), adjustedImpactPoint + (Vector3.right * colliderRadius)); // Linea verticale
        }
    }
}
