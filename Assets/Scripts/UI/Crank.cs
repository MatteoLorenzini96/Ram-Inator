using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Crank : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform parentRectTransform; // Il RectTransform del padre.
    private Vector2 previousDragPosition; // Per memorizzare la posizione precedente del mouse/touch.
    private WreckingBallController wreckingBallController;
    private float previousRotationStep = 0f; // Per tenere traccia dei cambi di -30°.
    private float currentRotationZ = 0f; // Rotazione attuale del padre.
    private Vector2 initialMousePosition; // Posizione iniziale del mouse.

    void Start()
    {
        // Trova automaticamente il RectTransform del padre.
        parentRectTransform = transform.parent.GetComponent<RectTransform>();

        if (parentRectTransform == null)
        {
            Debug.LogError("Il padre non ha un RectTransform!");
        }

        // Trova lo script WreckingBallController nella scena.
        wreckingBallController = FindFirstObjectByType<WreckingBallController>();
        if (wreckingBallController == null)
        {
            Debug.LogError("WreckingBallController non trovato nella scena!");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Memorizza la posizione iniziale quando inizia il drag.
        previousDragPosition = eventData.position;
        initialMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (parentRectTransform == null)
            return;

        // Calcola la differenza tra la posizione iniziale e quella attuale del mouse.
        Vector2 currentMousePosition = eventData.position;

        // Estrai solo la parte 2D della posizione dell'oggetto (x, y), ignorando il componente Z.
        Vector2 currentRectPosition = parentRectTransform.position; // Questa è un Vector3, quindi ne estraiamo solo la parte 2D
        Vector2 initialRectPosition = initialMousePosition; // Anche questa è un Vector2 (già corretta per il calcolo)

        // Calcoliamo l'angolo tra la posizione iniziale e la posizione attuale del mouse rispetto al centro dell'oggetto.
        Vector2 direction = currentMousePosition - currentRectPosition; // Direzione dal centro dell'oggetto alla posizione corrente del mouse
        Vector2 initialDirection = initialMousePosition - currentRectPosition; // Direzione dal centro dell'oggetto alla posizione iniziale del mouse

        float angle = Vector2.SignedAngle(initialDirection, direction); // Calcoliamo l'angolo tra le due direzioni

        // Applichiamo l'angolo alla rotazione dell'oggetto.
        currentRotationZ += angle;

        // Limita la rotazione tra 0° e -360°.
        currentRotationZ = Mathf.Clamp(currentRotationZ, -360f, 0f);

        // Aggiorna la rotazione del padre.
        parentRectTransform.rotation = Quaternion.Euler(0, 0, currentRotationZ);

        // Aggiorna il valore di `distance` ogni -30°.
        float step = Mathf.Floor(currentRotationZ / -30f) * -30f;

        if (step < previousRotationStep)
        {
            // Aumenta distance di 0.5.
            if (wreckingBallController != null)
                wreckingBallController.distance += 0.5f;
        }
        else if (step > previousRotationStep)
        {
            // Diminuisci distance di 0.5.
            if (wreckingBallController != null && wreckingBallController.distance > 0)
                wreckingBallController.distance -= 0.5f;
        }

        previousRotationStep = step; // Aggiorna il passo precedente.
        previousDragPosition = currentMousePosition; // Aggiorna la posizione precedente.
        initialMousePosition = currentMousePosition; // Aggiorna la posizione iniziale del mouse.
    }
}
