using UnityEngine;

public class ImpactHead : MonoBehaviour
{
    private ImpactHeadScript ImpactHeadScript;

    private void Start()
    {
        ImpactHeadScript = FindFirstObjectByType<ImpactHeadScript>();
    }

    // Metodo chiamato quando un oggetto entra nel trigger
    private void OnTriggerEnter(Collider other)
    {
        // Controlla se l'oggetto che entra nel trigger ha il tag "DestroyOnImpact"
        if (other.CompareTag("DestroyOnImpact"))
        {
            float relativeSpeed = ImpactHeadScript.impactHeadVelocity;

            // Prova a ottenere il componente CollisionStateChanger dall'oggetto
            CollisionStateChanger collisionStateChanger = other.GetComponent<CollisionStateChanger>();

            // Calcola il punto di impatto
            Vector3 impactPoint = other.ClosestPoint(transform.position);

            // Prova a ottenere il componente ParentCollisionManager dall'oggetto
            ParentCollisionManager parentCollisionManager = other.GetComponent<ParentCollisionManager>();

            // Se il componente CollisionStateChanger è stato trovato
            if (collisionStateChanger != null)
            {
                // Imposta la vita dell'oggetto a 0
                collisionStateChanger.Explode(relativeSpeed);
            }

            // Se il componente ParentCollisionManager è stato trovato
            if (parentCollisionManager != null)
            {
                // Passa il punto d'impatto alla funzione SetImpactPoint
                parentCollisionManager.SetImpactPoint(impactPoint);
            }

            // Puoi aggiungere altre logiche, come effetti visivi o suoni
            //Debug.Log($"Oggetto {other.gameObject.name} ha avuto impatto, punto: {impactPoint}");
        }
    }
}