using UnityEngine;
using UnityEngine.UI;

public class DistanceSliderController : MonoBehaviour
{
    [Header("Riferimenti")]
    private WreckingBallController wreckingBallController; // Riferimento al controller
    public Slider distanceSlider; // Riferimento allo slider
    private float initialDistance; // Distanza iniziale presa dall'inspector

    void Start()
    {

        // Trova lo script WreckingBallController nella scena.
        wreckingBallController = FindFirstObjectByType<WreckingBallController>();
        if (wreckingBallController == null)
        {
            Debug.LogError("WreckingBallController non trovato nella scena!");
        }

        if (wreckingBallController != null)
        {
            // Ottieni il valore iniziale della distanza
            initialDistance = wreckingBallController.distance;

            // Configura lo slider
            if (distanceSlider != null)
            {
                distanceSlider.minValue = initialDistance;
                distanceSlider.maxValue = initialDistance + 6;
                distanceSlider.value = initialDistance;

                // Aggiungi un listener per rilevare i cambiamenti nello slider
                distanceSlider.onValueChanged.AddListener(OnSliderValueChanged);
            }
        }
    }

    // Metodo chiamato quando cambia il valore dello slider
    void OnSliderValueChanged(float value)
    {
        if (wreckingBallController != null)
        {
            wreckingBallController.distance = value;
        }
    }
}
