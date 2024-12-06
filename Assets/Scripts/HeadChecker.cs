using UnityEngine;

public class HeadChecker : MonoBehaviour
{
    [Header("Prefab di Sostituzione")]
    public GameObject replacementPrefab;

    [Header("Tipi di Head Distruttivi")]
    public string[] destroyableHeads;

    private void OnCollisionEnter(Collision collision)
    {
        // Controlla se l'oggetto che ha colpito ha il tag corretto
        if (collision.gameObject.CompareTag("Palla"))
        {
            //Debug.Log("Impatto avvenuto con: " + collision.gameObject.name);

            // Cerca nei figli dell'oggetto con il tag "Palla"
            foreach (Transform child in collision.transform)
            {
                //Debug.Log("Controllando il figlio: " + child.name + ", Attivo: " + child.gameObject.activeSelf);

                if (child.gameObject.activeSelf)
                {
                    // Controlla se il figlio attivo è nella lista dei tipi distruttivi
                    if (System.Array.Exists(destroyableHeads, head => head == child.name))
                    {
                        //Debug.Log("La testa è corretta: " + child.name);

                        // Sostituisci l'oggetto con il prefab
                        if (replacementPrefab != null)
                        {
                            Instantiate(replacementPrefab, transform.position, transform.rotation);
                            Destroy(gameObject); // Distruggi l'oggetto attuale
                        }
                        return;
                    }
                    else
                    {
                        Debug.Log("La testa attiva non è distruttiva: " + child.name);
                    }
                }
            }

            Debug.Log("Nessun figlio attivo corrisponde ai tipi distruttivi.");
        }
    }
}
