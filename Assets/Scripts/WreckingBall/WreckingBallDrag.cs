using UnityEngine;
using System; // Necessario per gli eventi

public class WreckingBallDrag : MonoBehaviour
{
    private WreckingBallController controller;

    private Vector3 lastPosition; // Ultima posizione aggiornata
    private Vector3 currentVelocity; // Velocità attuale
    public bool isDragging = false;
    private Rigidbody rb;
    private Transform pivot;
    private Vector3 initialPosition; // Posizione iniziale della palla
    private Vector3 initialAnchorPosition; // Posizione iniziale del pivot
    public bool isSwinging = false;

    private Vector3 resetCheckStartPosition; // Posizione all'inizio del controllo di spostamento
    private float resetCheckStartTime; // Tempo all'inizio del controllo di spostamento

    public event Action<Vector3> OnRelease;

    [Header("Timer Per il reset")]
    public float resetTime = 2.0f; // Tempo di delay per il reset dopo il controllo

    [Header("Impostazioni di Spostamento")]
    public float minMovementToReset = 0.2f; // Spostamento minimo per evitare il reset
    public float movementCheckInterval = 1.0f; // Intervallo di tempo per il controllo dello spostamento

    [Header("Layer per le collisioni")]
    public LayerMask collisionLayer; // Definisce quale layer il raycast deve rilevare

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

        collisionLayer = LayerMask.GetMask("Muro"); // Assicurati che il layer "Muro" esista e sia configurato in Unity
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 touchPosition = GetTouchWorldPosition();

            Vector3 direction = touchPosition - rb.position;
            float distance = direction.magnitude;

            if (Physics.Raycast(rb.position, direction.normalized, out RaycastHit hit, distance, collisionLayer))
            {
                touchPosition = hit.point - direction.normalized * 0.1f; // Aggiungi un margine di distanza
            }

            rb.MovePosition(touchPosition);
            currentVelocity = (rb.position - lastPosition) / Time.deltaTime;
            lastPosition = rb.position;
        }
        else if (isSwinging)
        {
            // Controllo dello spostamento
            if (Time.time - resetCheckStartTime > movementCheckInterval)
            {
                float distanceMoved = Vector3.Distance(rb.position, resetCheckStartPosition);

                if (distanceMoved < minMovementToReset)
                {
                    //Debug.Log($"Resetting because moved only {distanceMoved} in {movementCheckInterval} seconds");
                    isSwinging = false; // Evita ulteriori reset
                    Invoke("ResetPosition", resetTime);
                }
                else
                {
                    // Aggiorna i valori per un nuovo controllo
                    resetCheckStartPosition = rb.position;
                    resetCheckStartTime = Time.time;
                }
            }
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
            gameObject.layer = LayerMask.NameToLayer("NoContact");
            isDragging = true;
            lastPosition = transform.position;
            initialPosition = transform.position;
            initialAnchorPosition = pivot.position;
        }
    }

    public void StopDragging()
    {
        if (isDragging)
        {
            OnRelease?.Invoke(currentVelocity);
            gameObject.layer = LayerMask.NameToLayer("Default");
            isDragging = false;
            isSwinging = true;

            // Inizia il controllo di spostamento
            resetCheckStartPosition = rb.position;
            resetCheckStartTime = Time.time;
        }
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
        TurnManager turnManager = FindFirstObjectByType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.OnReset();
        }
        else
        {
            Debug.LogWarning("TurnManager non trovato, impossibile chiamare OnStopDragging.");
        }

        transform.position = initialPosition;
        pivot.position = initialAnchorPosition;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.layer = LayerMask.NameToLayer("NoContact");
        //Debug.Log("Palla resettata alla posizione iniziale.");
        isSwinging = false;
    }
}
