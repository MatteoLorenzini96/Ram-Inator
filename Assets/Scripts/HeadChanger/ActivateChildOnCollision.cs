using UnityEngine;

public class AttivaFiglioOnCollision : MonoBehaviour
{
    public int indiceFiglioDaAttivare = 0; // Indice del figlio da attivare (modificabile nell'Inspector)

    [Header("Nome del SoundEffect da riprodurre")]
    public string soundEffectName = "SpikeStar"; // Variabile pubblica per modificare il nome del suono dall'Inspector

    private GameObject[] figli; // Lista dinamica dei figli dell'oggetto con cui si collide

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se l'oggetto con cui collide ha il tag "Palla"
        if (other.CompareTag("Palla"))
        {
            // Ottieni tutti i figli dell'oggetto con cui c'è stata la collisione
            Transform oggettoColliso = other.transform;
            figli = new GameObject[oggettoColliso.childCount];

            for (int i = 0; i < oggettoColliso.childCount; i++)
            {
                figli[i] = oggettoColliso.GetChild(i).gameObject;
            }

            //Debug.Log($"Figli trovati nell'oggetto colliso: {figli.Length}");

            // Assicurati che l'indice sia valido
            if (indiceFiglioDaAttivare >= 0 && indiceFiglioDaAttivare < figli.Length)
            {
                // Disattiva tutti i figli
                foreach (GameObject figlio in figli)
                {
                    //Debug.Log("Spenti tutti i figli dell'oggetto colliso");
                    if (figlio != null)
                        figlio.SetActive(false);
                }

                // Attiva il figlio specificato
                if (figli[indiceFiglioDaAttivare] != null)
                {
                    //Debug.Log("Accendo il figlio corretto dell'oggetto colliso");
                    figli[indiceFiglioDaAttivare].SetActive(true);
                }

                if (figli[3] != null)
                {
                    // Lascia sempre attivo il quarto figlio
                    //Debug.Log("Lasciando attivo il figlio con indice 4");
                    figli[3].SetActive(true);
                }

                // Distrugge l'oggetto a cui è assegnato lo script
                AudioManager.Instance.PlaySFX(soundEffectName); // Usa la variabile per chiamare il metodo
                Destroy(gameObject); // Distrugge l'oggetto corrente
            }
            else
            {
                Debug.LogWarning("Indice del figlio da attivare non valido!");
            }
        }
    }
}
