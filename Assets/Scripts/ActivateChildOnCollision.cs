using UnityEngine;

public class AttivaFiglioOnCollision : MonoBehaviour
{
    public GameObject[] figli; // Lista dei figli
    public int indiceFiglioDaAttivare = 0; // Indice del figlio da attivare (modificabile nell'Inspector)

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se l'oggetto con cui collide ha il tag "Palla"
        if (other.CompareTag("Palla"))
        {
            // Assicurati che l'indice sia valido
            if (indiceFiglioDaAttivare >= 0 && indiceFiglioDaAttivare < figli.Length)
            {
                // Disattiva tutti i figli
                foreach (GameObject figlio in figli)
                {
                    if (figlio != null)
                        figlio.SetActive(false);
                }

                // Attiva il figlio specificato
                if (figli[indiceFiglioDaAttivare] != null)
                {
                    figli[indiceFiglioDaAttivare].SetActive(true);
                }

                // Distrugge l'oggetto a cui è assegnato lo script
                Destroy(gameObject); // Distrugge l'oggetto corrente
            }
            else
            {
                Debug.LogWarning("Indice del figlio da attivare non valido!");
            }
        }
    }
}
