using UnityEngine;

public class AttivaFiglioOnCollision : MonoBehaviour
{
    public int indiceFiglioDaAttivare = 0; // Indice del figlio da attivare
    public string soundEffectName = "SpikeStar"; // Nome del sound effect
    private Transform oggettoColliso;

    // Riferimento allo script WreckingBallWeight_Velocity
    private WreckingBallWeight_Velocity weightVelocityManager;

    private void Start()
    {
        weightVelocityManager = FindFirstObjectByType<WreckingBallWeight_Velocity>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Palla"))
        {
            oggettoColliso = other.transform;

            // Disattiva tutti i figli tranne il quarto
            for (int i = 0; i < oggettoColliso.childCount; i++)
            {
                var child = oggettoColliso.GetChild(i).gameObject;
                child.SetActive(i == indiceFiglioDaAttivare || i == 3);
            }

            // Notifica il cambiamento al WreckingBallWeight_Velocity
            if (weightVelocityManager != null)
            {
                weightVelocityManager.UpdateRigidbodyProperties(indiceFiglioDaAttivare);
            }

            // Riproduci suono e distruggi l'oggetto corrente
            AudioManager.Instance.PlaySFX(soundEffectName);
            Destroy(gameObject);
        }
    }
}
