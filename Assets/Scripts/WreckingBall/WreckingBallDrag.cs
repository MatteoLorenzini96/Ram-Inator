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
    private Vector3 initialPosition; // Posizione iniziale della palla
    private Vector3 initialAnchorPosition; // Posizione iniziale del pivot
    private bool isSwinging = false;

    public event Action<Vector3> OnRelease;
    [Header("Tiper Per il reset")]
    public float resetTime = 2.0f;

    [Header("Impostazioni di Velocità")]
    public float minVelocityToReset = 0.5f; // Velocità minima per considerare il reset

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
        else if (rb != null && rb.linearVelocity.magnitude < minVelocityToReset && isSwinging == true)
        {
            Invoke("ResetPosition", resetTime);
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
        if (!isSwinging)
        {
            // Cambia il layer dell'oggetto in "NoContact"
            gameObject.layer = LayerMask.NameToLayer("NoContact");

            isDragging = true;

            lastPosition = transform.position;
            initialPosition = transform.position; // Salva la posizione iniziale della palla
            initialAnchorPosition = pivot.position; // Salva la posizione iniziale del pivot
        }
    }

    public void StopDragging()
    {
        OnRelease?.Invoke(currentVelocity);

        // Riporta il layer dell'oggetto a "Default"
        gameObject.layer = LayerMask.NameToLayer("Default");

        isDragging = false;
        isSwinging = true;
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

    private void ResetPosition()
    {
        transform.position = initialPosition; // Resetta la posizione della palla
        pivot.position = initialAnchorPosition; // Resetta la posizione dell'anchor
        rb.linearVelocity = Vector3.zero; // Resetta la velocità
        rb.angularVelocity = Vector3.zero; // Resetta la velocità angolare
        Debug.Log("Palla resettata alla posizione iniziale.");

        isSwinging = false;
    }
}