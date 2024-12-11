using UnityEngine;

public class WreckingBallPhysics : MonoBehaviour
{
    public float releaseForceMultiplier = 10.0f; // Moltiplicatore per calcolare la velocità al rilascio
    private Rigidbody rb;

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
    }

    private void ApplyReleaseForce(Vector3 releaseVelocity)
    {
        if (rb != null)
        {
            // Applica la velocità al Rigidbody
            rb.isKinematic = false;
            //Debug.Log($"Release Velocity: {releaseVelocity}");
            rb.linearVelocity = releaseVelocity * releaseForceMultiplier; // Imposta la velocità direttamente
        }
    }
}
