using UnityEngine;
using UnityEngine.EventSystems;

public class Crank : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float rotationSpeed = 200f; // Velocità di rotazione
    private bool isDragging = false;

    private float currentRotation = 0f; // Rotazione attuale dell'oggetto
    private float startingDistance; // Valore iniziale di "distance"

    private WreckingBallController wreckingBallController; // Riferimento allo script WreckingBallController

    private void Start()
    {
        // Trova lo script WreckingBallController in scena
        wreckingBallController = FindObjectOfType<WreckingBallController>();

        if (wreckingBallController == null)
        {
            Debug.LogError("WreckingBallController non trovato nella scena.");
            return;
        }

        // Salva il valore iniziale di "distance"
        startingDistance = wreckingBallController.distance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Inizia il drag
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Termina il drag
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (wreckingBallController == null)
            return;

        // Calcola la rotazione in base al movimento del mouse o del touch
        float rotationDelta = eventData.delta.x * rotationSpeed * Time.deltaTime;

        // Aggiorna la rotazione dell'immagine UI
        currentRotation += rotationDelta;
        currentRotation = Mathf.Clamp(currentRotation, 0, 360); // Limita la rotazione tra 0 e 360 gradi
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);

        // Aggiorna la variabile "distance" a intervalli di 30 gradi
        int steps = Mathf.FloorToInt(currentRotation / 30);
        wreckingBallController.distance = startingDistance + steps * 0.5f;
    }
}
