using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class WreckingBallDrag : MonoBehaviour
{
    public Button actionButton;
    private WreckingBallController controller;
    private SpikeAttached spikeAttached;
    private GameObject spawnedEffect;
    private WreckingBallWeight_Velocity wreckingBallVelocityManager;

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
    private Camera mainCamera;

    public event Action<Vector3> OnRelease;
    public event Action OnResetPosition;

    [Header("Timer Per il reset")]
    public float resetTime = 2.0f; // Tempo di delay per il reset dopo il controllo

    [Header("Impostazioni di Spostamento")]
    public float minMovementToReset = 0.2f; // Spostamento minimo per evitare il reset
    public float movementCheckInterval = 1.0f; // Intervallo di tempo per il controllo dello spostamento

    [Header("Layer per le collisioni")]
    public LayerMask collisionLayer; // Definisce quale layer il raycast deve rilevare

    [Header("Impostazioni di Setting")]
    public float initialSettingDuration = 4.0f; // Durata della finestra temporale per il setting iniziale

    [Header("VFX da riprodurre")]
    public string settingEffectName; // Nome dell'effetto da instanziare quando finisce la finestra

    [Header("SoundEffect da riprodurre")]
    public string soundEffectActivate; // Variabile pubblica per modificare il nome del suono dall'Inspector

    private bool isSetting = false; // Indica se siamo nella finestra di setting iniziale

    void Start()
    {
        SearchSpriptsAndRb();
        UpdateButtonState();

        collisionLayer = LayerMask.GetMask("Muro"); // Assicurati che il layer "Muro" esista e sia configurato in Unity
    }

    private void SearchSpriptsAndRb()
    {
        rb = GetComponent<Rigidbody>();

        mainCamera = Camera.main;

        controller = GetComponent<WreckingBallController>();
        if (controller == null)
        {
            Debug.LogError("WreckingBallController non trovato. Aggiungilo all'oggetto.");
        }

        // Cerca il SpikeAttached
        if (spikeAttached == null)
        {
            spikeAttached = FindFirstObjectByType<SpikeAttached>();
            if (spikeAttached == null)
            {
                Debug.LogError("Non è stato trovato un oggetto con SpikeAttached nella scena!");
            }
            else
            {
                spikeAttached.OnDetach += HandleDetach; // Iscriviti all'evento
            }
        }

        pivot = GetComponentInParent<AutoConfigurableJoint>().pivot;
        if (pivot == null)
        {
            Debug.LogError("Pivot non trovato, assicurati che AutoConfigurableJoint sia configurato correttamente.");
        }

        wreckingBallVelocityManager = FindFirstObjectByType<WreckingBallWeight_Velocity>();
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            HandleDragging();
        }
        else if (isSwinging)
        {
            // Controllo dello spostamento
            if (Time.time - resetCheckStartTime > movementCheckInterval)
            {
                CheckSwingReset();
            }
        }
    }

    private void CheckSwingReset()
    {
        float distanceMoved = Vector3.Distance(rb.position, resetCheckStartPosition);

        if (distanceMoved < minMovementToReset)
        {
            //Debug.Log($"Resetting because moved only {distanceMoved} in {movementCheckInterval} seconds");
            if (!spikeAttached.isSpikeAttached)
            {
                isSwinging = false; // Evita ulteriori reset
                Invoke("ResetPosition", resetTime);
            }
        }
        else
        {
            // Aggiorna i valori per un nuovo controllo
            resetCheckStartPosition = rb.position;
            resetCheckStartTime = Time.time;
        }
    }

    void HandleDragging()
    {
        // Ottieni la posizione del mouse/tocco nel mondo
        Vector3 touchPosition = GetTouchWorldPosition();

        // Direzione e distanza dal pivot
        Vector3 direction = touchPosition - pivot.position;
        float distanceToPivot = direction.magnitude;

        // Limita la distanza massima dal pivot
        if (controller != null && distanceToPivot > controller.distance)
        {
            direction = direction.normalized * controller.distance;
            touchPosition = pivot.position + direction;
        }

        // Calcola la posizione desiderata
        Vector3 targetPosition = touchPosition;

        // Calcola la velocità basata sulla posizione target
        Vector3 desiredVelocity = (targetPosition - rb.position) / Time.fixedDeltaTime;

        // Limita la velocità per evitare movimenti eccessivi
        float maxSpeed = wreckingBallVelocityManager.GetCurrentMaxSpeed(); // Definisci una velocità massima nel controller
        if (desiredVelocity.magnitude > maxSpeed)
        {
            desiredVelocity = desiredVelocity.normalized * maxSpeed;
        }

        // Applica la velocità al Rigidbody
        rb.linearVelocity = desiredVelocity;

        // Aggiorna posizione e velocità per altre logiche
        currentVelocity = rb.linearVelocity;
        lastPosition = rb.position;
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

            // Avvia la finestra di setting iniziale
            isSetting = true;
            Invoke("EndInitialSetting", initialSettingDuration);

            // Spawna l'effetto nella posizione corrente e lo assegna come figlio dell'oggetto corrente
            spawnedEffect = EffectsManager.Instance?.SpawnEffect(settingEffectName, transform.position);
            if (spawnedEffect != null)
            {
                spawnedEffect.transform.SetParent(this.transform, true); // Imposta come figlio mantenendo la posizione mondiale
            }
        }
    }

    public void StopDragging()
    {
        if (isDragging)
        {
            if (isSetting)
            {
                ResetPosition();
                isSetting = false; // Fine del setting iniziale
            }
            else
            {
                OnRelease?.Invoke(currentVelocity);
                gameObject.layer = LayerMask.NameToLayer("Default");
                isDragging = false;
                isSwinging = true;

                // Aggiorna lo stato del bottone
                UpdateButtonState();

                // Inizia il controllo di spostamento
                resetCheckStartPosition = rb.position;
                resetCheckStartTime = Time.time;
            }
        }
    }

    private Vector3 GetTouchWorldPosition()
    {
        Vector3 inputPosition;

        if (Input.touchCount > 0)
        {
            // Usa la posizione del primo tocco attivo
            inputPosition = Input.GetTouch(0).position;
        }
        else if (Input.GetMouseButton(0))
        {
            // Fallback al mouse per il debug su PC
            inputPosition = Input.mousePosition;
        }
        else
        {
            return transform.position; // Se non ci sono input, restituisci la posizione corrente
        }

        // Converti la posizione dello schermo in posizione nel mondo
        inputPosition.z = mainCamera.WorldToScreenPoint(transform.position).z; // Mantieni la profondità
        return mainCamera.ScreenToWorldPoint(inputPosition);
    }


    public void ResetPosition()
    {
        // Annulla e riavvia il timer della finestra di setting iniziale
        if (isSetting)
        {
            CancelInvoke("EndInitialSetting"); // Annulla il timer corrente

            // Distruggi l'effetto se esiste
            if (spawnedEffect != null)
            {
                Destroy(spawnedEffect);
            }

            Invoke("EndInitialSetting", initialSettingDuration); // Riavvia il timer
            isDragging = false;
        }

        if (!spikeAttached.isSpikeAttached)
        {
            TurnManager turnManager = FindFirstObjectByType<TurnManager>();
            if (turnManager != null && !isSetting)
            {
                turnManager.OnReset();
            }
            if (turnManager = null)
            {
                Debug.LogWarning("TurnManager non trovato, impossibile chiamare OnReset.");
            }

            transform.position = initialPosition;
            pivot.position = initialAnchorPosition;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            gameObject.layer = LayerMask.NameToLayer("NoContact");

            isSwinging = false;

            // Aggiorna lo stato del bottone
            UpdateButtonState();
        }
        else
        {
            isSwinging = true;
            UpdateButtonState();
        }

        // Invoca l'evento
        OnResetPosition?.Invoke();
    }

    // Aggiorna la funzione HandleDetach per non interferire con la logica originale
    private void HandleDetach()
    {
        if (!isSwinging)
        {
            StartCoroutine(DelayedReset());
        }
    }

    private IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(3.0f); // Aspetta 3 secondi o il tempo desiderato

        // Solo se la velocità è minima e lo spike non è attaccato, effettua il reset
        if (!spikeAttached.isSpikeAttached && rb.linearVelocity.magnitude < minMovementToReset)
        {
            ResetPosition();
        }
    }

    private void UpdateButtonState()
    {
        if (actionButton != null)
        {
            actionButton.interactable = isSwinging;
        }
        else
        {
            Debug.LogWarning("Il riferimento al bottone non è stato assegnato.");
        }
    }

    private void EndInitialSetting()
    {
        isSetting = false;

        // Instanzia l'effetto quando finisce il setting iniziale
        if (!string.IsNullOrEmpty(settingEffectName))
        {
            AudioManager.Instance.PlaySFX(soundEffectActivate);
        }
    }
}
