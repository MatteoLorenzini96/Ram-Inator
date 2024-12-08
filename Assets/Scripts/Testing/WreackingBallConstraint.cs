using UnityEngine;

[RequireComponent(typeof(WreckingBallController), typeof(AutoConfigurableJoint))]
public class WreckingBallConstraint : MonoBehaviour
{
    private WreckingBallController wreckingBallController; // Riferimento al controller della palla demolitrice
    private AutoConfigurableJoint autoConfigurableJoint; // Riferimento al configuratore del joint

    private Transform pivot; // Punto Pivot
    private Rigidbody ballRigidbody; // Rigidbody della palla
    private float distance; // Raggio massimo consentito

    void Awake()
    {
        // Ottieni i riferimenti agli altri script
        wreckingBallController = GetComponent<WreckingBallController>();
        autoConfigurableJoint = GetComponent<AutoConfigurableJoint>();

        // Assicurati che i riferimenti siano validi
        if (wreckingBallController == null || autoConfigurableJoint == null)
        {
            Debug.LogError("Assicurati che WreckingBallController e AutoConfigurableJoint siano presenti sullo stesso GameObject.");
            return;
        }

        // Configura i riferimenti
        pivot = autoConfigurableJoint.pivot;
        ballRigidbody = GetComponent<Rigidbody>();

        if (pivot == null || ballRigidbody == null)
        {
            Debug.LogError("Assicurati che il Pivot sia assegnato in AutoConfigurableJoint e che il Rigidbody sia presente sullo stesso GameObject.");
            return;
        }

        // Ottieni il valore iniziale della distanza
        distance = wreckingBallController.distance;
    }

    void FixedUpdate()
    {
        if (pivot == null || ballRigidbody == null)
        {
            return;
        }

        // Aggiorna il valore della distanza da WreckingBallController
        distance = wreckingBallController.distance;

        // Calcola la distanza tra la palla e il pivot
        Vector3 offset = ballRigidbody.position - pivot.position;
        float currentDistance = offset.magnitude;

        // Controlla se la palla è oltre il raggio massimo consentito
        if (currentDistance > distance)
        {
            // Riduci la posizione della palla per riportarla entro il raggio massimo
            Vector3 correctedPosition = pivot.position + offset.normalized * distance;

            // Aggiorna la posizione della palla
            ballRigidbody.MovePosition(correctedPosition);
        }
    }
}
