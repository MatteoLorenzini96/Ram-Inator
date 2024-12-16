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
    private bool noTouch =false;

    public event Action<Vector3> OnRelease;

    [Header("Impostazioni di Velocità")]
    public float minVelocityToDrag = 1.0f; // Velocità minima per consentire il trascinamento

    void Start()
    {
        controller = GetComponent<WreckingBallController>();
        if (controller == null)
        {
            Debug.LogError("WreckingBallController non trovato. Aggiungilo all'oggetto.");
        }

        rb = GetComponent<Rigidbody>();
        pivot = GetComponentInParent<AutoConfigurableJoint>().pivot;
        if (pivot == null)
        {
            Debug.LogError("Pivot non trovato, assicurati che AutoConfigurableJoint sia configurato correttamente.");
        }

        lastPosition = transform.position;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 touchPosition = GetTouchWorldPosition();

            Vector3 direction = touchPosition - pivot.position;
            if (direction.magnitude > controller.distance)
            {
                touchPosition = pivot.position + direction.normalized * controller.distance;
            }

            transform.position = touchPosition;

            currentVelocity = (transform.position - lastPosition) / Time.deltaTime;

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
        // Controlla se la velocità attuale supera il limite
        if (rb != null && rb.linearVelocity.magnitude > minVelocityToDrag)
        {
            noTouch = true;
            Debug.Log("La palla si sta muovendo troppo velocemente per essere trascinata.");
            return; // Esci dalla funzione senza attivare il trascinamento
        }

        if (rb != null)
        {
            rb.isKinematic = true; // Disabilita la fisica per il trascinamento
            noTouch = false;
        }
        isDragging = true;
        lastPosition = transform.position;
    }

    public void StopDragging()
    {
        if (rb != null && noTouch == false)
        {
            rb.isKinematic = false;

            OnRelease?.Invoke(currentVelocity);
        }
        isDragging = false;
    }

    private Vector3 GetTouchWorldPosition()
    {
        Vector3 inputPosition;

        if (Input.touchCount > 0)
        {
            inputPosition = Input.GetTouch(0).position;
        }
        else if (Input.GetMouseButton(0))
        {
            inputPosition = Input.mousePosition;
        }
        else
        {
            return transform.position;
        }

        inputPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(inputPosition);
    }
}
