using UnityEngine;

public class ReplaceWithPrefab : MonoBehaviour
{
    // Il prefab che sostituirà questo oggetto
    public GameObject prefabToInstantiate;

    // Metodo chiamato quando avviene una collisione
    private void OnCollisionEnter(Collision collision)
    {
        // Controlla se l'oggetto che ha colpito ha il tag "ariete"
        if (collision.gameObject.CompareTag("ariete"))
        {
            // Ottieni la posizione e la rotazione attuali dell'oggetto
            Vector3 currentPosition = transform.position;
            Quaternion currentRotation = transform.rotation;

            // Instanzia il prefab nella stessa posizione e rotazione
            Instantiate(prefabToInstantiate, currentPosition, currentRotation);

            // Distruggi questo oggetto
            Destroy(gameObject);
        }
    }
}
