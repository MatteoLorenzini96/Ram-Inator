using UnityEngine;
using System.Collections.Generic;

public class SoundOnCollision : MonoBehaviour
{
    [Header("SoundEffect da riprodurre alla collisione con la Palla")]
    public string soundEffectNormalHeadHit;
    public string soundEffectSpikeHeadHit;
    public string soundEffectImpactHeadHit;

    // Dizionario per gestire i cooldown dei suoni
    private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();
    public float soundCooldown = 0.5f; // Cooldown globale per i suoni

    // Controlla se la collisione è già stata gestita per evitare duplicazioni
    private bool collisionHandled = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Controlla se l'oggetto in collisione ha il tag "Palla"
        if (collision.gameObject.CompareTag("Palla") && !collisionHandled)
        {
            // Flag per indicare che la collisione è gestita
            collisionHandled = true;

            // Itera tra i figli dell'oggetto per verificare quale è attivo
            foreach (Transform child in collision.gameObject.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    // Controlla il nome del figlio attivo e riproduci il suono corrispondente con cooldown
                    switch (child.gameObject.name)
                    {
                        case "NormalHead":
                            PlaySoundWithCooldown(soundEffectNormalHeadHit);
                            break;

                        case "SpikeHead":
                            PlaySoundWithCooldown(soundEffectSpikeHeadHit);
                            break;

                        case "ImpactHead":
                            PlaySoundWithCooldown(soundEffectImpactHeadHit);
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

    private void OnCollisionExit(Collision collision)
    {
        // Reset il flag quando la collisione termina
        if (collision.gameObject.CompareTag("Palla"))
        {
            collisionHandled = false;
        }
    }

    private void PlaySoundWithCooldown(string soundEffect)
    {
        float currentTime = Time.time;

        // Controlla se il suono ha un cooldown attivo
        if (soundCooldowns.ContainsKey(soundEffect))
        {
            if (currentTime - soundCooldowns[soundEffect] < soundCooldown)
            {
                // Il suono è ancora in cooldown
                return;
            }
        }

        // Aggiorna il tempo di ultima riproduzione
        soundCooldowns[soundEffect] = currentTime;

        // Riproduce il suono
        AudioManager.Instance.PlaySFX(soundEffect);
    }
}
