using UnityEngine;

public class AutoConfigurableJoint : MonoBehaviour
{
    [Header("Riferimenti")]
    public ConfigurableJoint joint; // Il ConfigurableJoint da configurare
    public Transform pivot; // Il punto pivot (un GameObject vuoto nella scena)

    void Start()
    {
        if (joint == null || pivot == null)
        {
            Debug.LogError("Assicurati di assegnare sia il ConfigurableJoint che il pivot.");
            return;
        }

        // Configura automaticamente il Connected Anchor in base al pivot
        ConfigureConnectedAnchor();
    }

    void Update()
    {
        if (joint != null && pivot != null)
        {
            // Aggiorna continuamente il Connected Anchor alla posizione del pivot
            joint.connectedAnchor = pivot.position;
        }
    }

    void ConfigureConnectedAnchor()
    {
        // Imposta il Connected Anchor alla posizione del pivot
        joint.connectedAnchor = pivot.position;

        Debug.Log("Connected Anchor configurato automaticamente in posizione: " + pivot.position);
    }
}
