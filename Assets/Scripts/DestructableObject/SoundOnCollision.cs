using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    [Header("SoundEffect da riprodurre alla collisione con la Palla")]
    public string soundEffectNormalHeadHit;
    public string soundEffectSpikeHeadHit;
    public string soundEffectImpactHeadHit;

    private void OnCollisionEnter(Collision collision)
    {
        // Controlla se l'oggetto in collisione ha il tag "Palla"
        if (collision.gameObject.CompareTag("Palla"))
        {
            // Itera tra i figli dell'oggetto per verificare quale � attivo
            foreach (Transform child in collision.gameObject.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    // Controlla il nome del figlio attivo e riproduci il suono corrispondente
                    switch (child.gameObject.name)
                    {
                        case "NormalHead":
                            AudioManager.Instance.PlaySFX(soundEffectNormalHeadHit);
                            break;

                        case "SpikeHead":
                            AudioManager.Instance.PlaySFX(soundEffectSpikeHeadHit);
                            break;

                        case "ImpactHead":
                            AudioManager.Instance.PlaySFX(soundEffectImpactHeadHit);
                            break;

                        default:
                            Debug.LogWarning("Nessun suono associato al figlio attivo: " + child.gameObject.name);
                            break;
                    }

                    // Esci dal ciclo una volta trovato il figlio attivo
                    break;
                }
            }
        }
    }
}