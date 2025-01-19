using UnityEngine;

public class WreckingBallPhysics : MonoBehaviour
{
    public float releaseForceMultiplier = 10.0f; // Moltiplicatore per calcolare la velocità al rilascio
    private Rigidbody rb;

    // Riferimento a WreckingBallWeight_Velocity per ottenere maxSpeed
    private WreckingBallWeight_Velocity weightVelocityManager;

    void Start()
    {
        // Recupera il Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody non trovato. Aggiungilo all'oggetto.");
        }

        // Collega l'evento di rilascio
        WreckingBallDrag dragScript = GetComponent<WreckingBallDrag>();
        if (dragScript != null)
        {
            dragScript.OnRelease += ApplyReleaseForce; // Sottoscrivi l'evento
        }
        else
        {
            Debug.LogError("WreckingBallDrag non trovato. Assicurati che lo script sia presente.");
        }

        // Recupera il riferimento a WreckingBallWeight_Velocity
        weightVelocityManager = GetComponent<WreckingBallWeight_Velocity>();
        if (weightVelocityManager == null)
        {
            Debug.LogError("WreckingBallWeight_Velocity non trovato. Assicurati che lo script sia presente.");
        }
    }

    private void ApplyReleaseForce(Vector3 releaseVelocity)
    {
        if (rb != null && weightVelocityManager != null)
        {
            // Ottieni la velocità massima attuale
            float maxSpeed = weightVelocityManager.GetCurrentMaxSpeed();

            // Calcola la velocità di rilascio
            Vector3 calculatedVelocity = releaseVelocity * releaseForceMultiplier;

            // Limita la velocità al valore massimo configurato
            if (calculatedVelocity.magnitude > maxSpeed)
            {
                calculatedVelocity = calculatedVelocity.normalized * maxSpeed;
            }

            // Applica la velocità limitata al Rigidbody
            rb.linearVelocity = calculatedVelocity;

            //Debug.Log($"Rilascio: velocità applicata = {calculatedVelocity.magnitude}, direzione = {calculatedVelocity.normalized}");
        }
    }
}
