using UnityEngine;

public class ImpactHead : MonoBehaviour
{
    // Metodo chiamato quando un oggetto entra nel trigger
    private void OnTriggerEnter(Collider other)
    {
        // Controlla se l'oggetto che entra nel trigger ha il tag "DestroyOnImpact"
        if (other.CompareTag("DestroyOnImpact"))
        {
            // Prova a ottenere il componente CollisionStateChanger dall'oggetto
            CollisionStateChanger collisionStateChanger = other.GetComponent<CollisionStateChanger>();

            // Se il componente CollisionStateChanger � stato trovato
            if (collisionStateChanger != null)
            {
                // Imposta la vita dell'oggetto a 0
                collisionStateChanger.Explode();

                // Puoi aggiungere altre logiche, come effetti visivi o suoni
                //Debug.Log($"Oggetto {other.gameObject.name} ha avuto impatto, vite ora: {collisionStateChanger.viteoggetto}");
            }
        }
    }
}
