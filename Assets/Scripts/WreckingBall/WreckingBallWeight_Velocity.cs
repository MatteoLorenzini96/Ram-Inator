using UnityEngine;

public class WreckingBallWeight_Velocity : MonoBehaviour
{
    private Rigidbody rb;
    private Transform child0, child1, child2;

    [Header("Valori NormalHead")]
    public float massNormal = 2f;
    public float dragNormal = 0.2f;
    public float maxSpeedNormal = 10f;

    [Header("Valori SpikeHead")]
    public float massSpike = 10f;
    public float dragSpike = 0.5f;
    public float maxSpeedSpike = 10f;

    [Header("Valori ImpactHead")]
    public float massImpact = 1f;
    public float dragImpact = 0f;
    public float maxSpeedImpact = 10f;


    void Start()
    {
        // Ottieni il componente Rigidbody sull'oggetto
        rb = GetComponent<Rigidbody>();

        // Controlla i figli di questo oggetto una sola volta, all'inizio
        child0 = transform.childCount > 0 ? transform.GetChild(0) : null;
        child1 = transform.childCount > 1 ? transform.GetChild(1) : null;
        child2 = transform.childCount > 2 ? transform.GetChild(2) : null;
    }

    void Update()
    {
        // Controlla lo stato di attivazione dei figli ogni frame e modifica le proprietà del Rigidbody
        if (child0 != null && child0.gameObject.activeSelf)
        {
            // Modifica le proprietà del Rigidbody se NormalHead è attivo
            rb.mass = massNormal;
            rb.linearDamping = dragNormal;
            LimitSpeed(maxSpeedNormal);
        }
        else if (child1 != null && child1.gameObject.activeSelf)
        {
            // Modifica le proprietà del Rigidbody se SpikeHead è attivo
            rb.mass = massSpike;
            rb.linearDamping = dragSpike;
            LimitSpeed(maxSpeedSpike);
        }
        else if (child2 != null && child2.gameObject.activeSelf)
        {
            // Modifica le proprietà del Rigidbody se ImpactHead è attivo
            rb.mass = massImpact;
            rb.linearDamping = dragImpact;
            LimitSpeed(maxSpeedImpact);
        }
    }

    // Funzione per limitare la velocità del Rigidbody
    void LimitSpeed(float maxSpeed)
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

}
