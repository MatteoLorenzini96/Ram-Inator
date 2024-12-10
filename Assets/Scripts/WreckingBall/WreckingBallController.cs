using UnityEngine;
using UnityEngine.UI;

public class WreckingBallController : MonoBehaviour
{
    [Header("Riferimenti")]
    public ConfigurableJoint joint; // Il ConfigurableJoint già configurato
    public float distance = 5f; // Distanza desiderata tra pivot e palla
    public Slider distanceSlider; // Slider per modificare la distanza

    void Start()
    {
        if (joint == null)
        {
            Debug.LogError("Devi assegnare un ConfigurableJoint nello script.");
            return;
        }

        if (distanceSlider == null)
        {
            Debug.LogError("Devi assegnare uno Slider nello script.");
            return;
        }

        // Configura la distanza iniziale del joint
        distanceSlider.value = distance; // Sincronizza lo slider con la distanza iniziale
        distanceSlider.onValueChanged.AddListener(UpdateDistanceFromSlider); // Ascolta i cambiamenti dello slider
        UpdateJointDistance();
    }

    void UpdateDistanceFromSlider(float value)
    {
        distance = value;
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
