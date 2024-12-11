using UnityEngine;
using UnityEngine.UI;

public class WreckingBallController : MonoBehaviour
{
    [Header("Riferimenti")]
    public ConfigurableJoint joint; // Il ConfigurableJoint già configurato
    public float distance = 5f; // Distanza desiderata tra pivot e palla

    void Start()
    {
        UpdateJointDistance();
    }

    private void Update()
    {
        UpdateJointDistance();
    }

    void UpdateJointDistance()
    {
        if (joint != null)
        {
            // Aggiorna il limite lineare del Hoint
            SoftJointLimit limit = new SoftJointLimit
            {
                limit = distance
            };
            joint.linearLimit = limit;
        }
    }
}
