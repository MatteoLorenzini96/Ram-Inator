using UnityEngine;
using System; // Necessario per gli eventi

public class WreckingBallDrag : MonoBehaviour
{
    private WreckingBallController controller;

    private Vector3 lastPosition; // Ultima posizione aggiornata
    private Vector3 currentVelocity; // Velocità attuale
    private bool isDragging = false;
    private Rigidbody rb;
    private Transform pivot;

    // Definisce un evento OnRelease per passare la velocità a WreckingBallPhysics
    public event Action<Vector3> OnRelease;

    void Start()
    {
        // Recupera lo script WreckingBallController
        controller = GetComponent<WreckingBallController>();
        if (controller == null)
        {
            Debug.LogError("WreckingBallController non trovato. Aggiungilo all'oggetto.");
        }

        rb = GetComponent<Rigidbody>();
        pivot = GetComponentInParent<AutoConfigurableJoint>().pivot; // Assicurati che il pivot sia corretto
        if (pivot == null)
        {
            Debug.LogError("Pivot non trovato, assicurati che AutoConfigurableJoint sia configurato correttamente.");
        }

        lastPosition = transform.position; // Imposta la posizione iniziale
    }

    void Update()
    {
        if (isDragging)
        {
            // Rileva la posizione di tocco o mouse
            Vector3 touchPosition = GetTouchWorldPosition();

            // Limita la posizione alla distanza massima dal pivot
            Vector3 direction = touchPosition - pivot.position;
            if (direction.magnitude > controller.distance)
            {
                touchPosition = pivot.position + direction.normalized * controller.distance;
            }

            // Sposta l'oggetto alla nuova posizione
            transform.position = touchPosition;

            // Calcola la velocità se la posizione è cambiata
            currentVelocity = (transform.position - lastPosition) / Time.deltaTime;

            // Salva la posizione corrente per il prossimo frame
            lastPosition = transform.position;
        }
    }

    void OnMouseDown()
    {
        StartDragging();
    }

    void OnMouseUp()
    {
        StopDragging();
    }

    private void StartDragging()
    {
        if (rb != null)
        {
            rb.isKinematic = true; // Disabilita la fisica per il trascinamento
        }
        isDragging = true;
        lastPosition = transform.position; // Inizia a tracciare la posizione
    }

    public void StopDragging()
    {
        if (rb != null)
        {
            rb.isKinematic = false; // Riabilita la fisica al rilascio

            // Se l'evento OnRelease è stato sottoscritto, invoca l'evento
            OnRelease?.Invoke(currentVelocity); // Passa la velocità al listener (WreckingBallPhysics)
        }
        isDragging = false;
    }

    private Vector3 GetTouchWorldPosition()
    {
        Vector3 inputPosition;

        if (Input.touchCount > 0) // Rileva il tocco
        {
            inputPosition = Input.GetTouch(0).position;
        }
        else if (Input.GetMouseButton(0)) // Rileva il click del mouse
        {
            inputPosition = Input.mousePosition;
        }
        else
        {
            return transform.position;
        }

        // Converte la posizione dello schermo in coordinate del mondo
        inputPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(inputPosition);
    }
}
