using UnityEngine;
using UnityEngine.UI;

public class WreckingBallController : MonoBehaviour
{
    [Header("Riferimenti")]
    public ConfigurableJoint joint; // Il ConfigurableJoint già configurato
    public float distance = 5f; // Distanza desiderata tra pivot e palla

    void Start()
    {
        if (joint == null)
        {
            Debug.LogError("Devi assegnare un ConfigurableJoint nello script.");
            return;
        }

        UpdateJointDistance();
    }

 
    void UpdateJointDistance()
    {
        if (joint != null)
        {
            SoftJointLimit limit = new SoftJointLimit
            {
                limit = distance
            };
            joint.linearLimit = limit;
        }
    }
}
