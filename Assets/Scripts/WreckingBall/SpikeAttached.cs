using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Per usare la lista
using System;
using UnityEngine.InputSystem.XR;

public class SpikeAttached : MonoBehaviour
{
    [SerializeField] private Transform parentObject; // Il parent che contiene i child
    private int activeChildIndex = 1; // L'indice del child da controllare
    private WreckingBallDrag wreckingBallDrag;
    public string targetTag = "TargetTag"; // Il tag dell'oggetto a cui agganciarsi
    public float attachDuration = 5f; // Durata dell'attaccamento in secondi
    private Transform activeChild;
    private bool isScriptActive = false; // Stato di attivazione script
    private Coroutine attachCoroutine;
    public bool isSpikeAttached = false;
    public event Action OnDetach; // Evento da chiamare quando lo spike si stacca

    private HashSet<GameObject> temporarilyBlockedObjects = new HashSet<GameObject>(); // Oggetti temporaneamente bloccati
    public float reattachCooldown = 3f; // Tempo di attesa prima di potersi riattaccare

    private void Start()
    {
        // Assicurati che il parentObject sia assegnato
        if (parentObject == null)
        {
            Debug.LogError("Parent Object non assegnato! Assegna un parent nel campo 'Parent Object' nell'Inspector.");
            return;
        }

        // Ottieni il child al determinato indice
        if (parentObject.childCount > activeChildIndex)
        {
            activeChild = parentObject.GetChild(activeChildIndex);
        }
        else
        {
            Debug.LogError($"Il parent non ha abbastanza child. Assicurati che ci sia un child all'indice {activeChildIndex}.");
        }

        wreckingBallDrag = GetComponent<WreckingBallDrag>();
        if (wreckingBallDrag == null)
        {
            Debug.LogError("WreckingBallDrag non trovato. Aggiungilo all'oggetto.");
        }
    }

    private void Update()
    {
        if (activeChild != null)
        {
            bool isActive = activeChild.gameObject.activeSelf;

            if (isActive && !isScriptActive)
            {
                isScriptActive = true;
            }
            else if (!isActive && isScriptActive)
            {
                isScriptActive = false;
                if (attachCoroutine != null)
                {
                    StopCoroutine(attachCoroutine);
                }
            }
        }
        else
        {
            Debug.Log("Active child non trovato o è null.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Esegui il codice solo se lo script è attivo e il child corretto è attivo
        if (isScriptActive && collision.gameObject.CompareTag(targetTag) && wreckingBallDrag.isSwinging)
        {
            // Controlla se l'oggetto è temporaneamente bloccato
            if (temporarilyBlockedObjects.Contains(collision.gameObject))
            {
                Debug.Log("Oggetto temporaneamente bloccato, impossibile attaccarsi.");
                return;
            }

            if (activeChild != null && activeChild.gameObject.activeSelf)
            {
                if (attachCoroutine == null)
                {
                    attachCoroutine = StartCoroutine(AttachToTarget(collision.gameObject));
                }
            }
            else
            {
                Debug.LogWarning("Il child attivo non è quello corretto. Nessun attaccamento eseguito.");
            }
        }
    }

    private IEnumerator AttachToTarget(GameObject target)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Salva la posizione e rotazione relative iniziali
        Vector3 initialPositionOffset = transform.position - target.transform.position;
        Quaternion initialRotationOffset = Quaternion.Inverse(target.transform.rotation) * transform.rotation;

        float timer = 0f;
        while (timer < attachDuration)
        {
            isSpikeAttached = true;

            if (rb != null)
            {
                // Disattiva la fisica durante l'attaccamento
                rb.isKinematic = true;
            }

            // Mantieni la posizione e la rotazione relative durante l'attaccamento
            transform.position = target.transform.position + initialPositionOffset;
            transform.rotation = target.transform.rotation * initialRotationOffset;

            timer += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
        {
            // Riattiva la fisica
            rb.isKinematic = false;
        }

        // Disattiva l'agganciamento dopo il tempo specificato
        attachCoroutine = null;
        isSpikeAttached = false;

        // Blocca temporaneamente l'oggetto
        TemporarilyBlockObject(target);

        OnDetach?.Invoke(); // Emetti l'evento
    }

    private void TemporarilyBlockObject(GameObject target)
    {
        if (!temporarilyBlockedObjects.Contains(target))
        {
            temporarilyBlockedObjects.Add(target);
            StartCoroutine(UnblockObjectAfterDelay(target));
        }
    }

    private IEnumerator UnblockObjectAfterDelay(GameObject target)
    {
        yield return new WaitForSeconds(reattachCooldown);

        if (temporarilyBlockedObjects.Contains(target))
        {
            temporarilyBlockedObjects.Remove(target);
            Debug.Log($"Oggetto sbloccato: {target.name}");
        }
    }
}
