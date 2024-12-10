using UnityEngine;

public class WreckingBallController : MonoBehaviour
{
    [Header("Riferimenti")]
    public ConfigurableJoint joint; // Il ConfigurableJoint gi� configurato
    public float distance = 5f; // Distanza desiderata tra pivot e palla
    

    void Start()
    {
        if (joint == null)
        {
            Debug.LogError("Devi assegnare un ConfigurableJoint nello script.");
            return;
        }

        // Configura la distanza iniziale del joint
        UpdateJointDistance();
    }

    void Update()
    {
        // Modifica la distanza del joint in tempo reale
        UpdateJointDistance();
    }
    void UpdateJointDistance()
    {
        if (joint != null)
        {
            // Aggiorna il limite lineare del joint
            SoftJointLimit limit = new SoftJointLimit
            {
                limit = distance
            };
            joint.linearLimit = limit;
        }
    }
}
