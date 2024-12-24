using UnityEngine;
using System.Collections;

public class SpikeAttached : MonoBehaviour
{
    [SerializeField] private Transform parentObject; // Il parent che contiene i child
    private int activeChildIndex = 1; // L'indice del child da controllare
    public string targetTag = "TargetTag"; // Il tag dell'oggetto a cui agganciarsi
    public float attachDuration = 5f; // Durata dell'attaccamento in secondi
    private Transform activeChild;
    private bool isScriptActive = false; // Stato di attivazione script
    private Coroutine attachCoroutine;

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
        if (isScriptActive && collision.gameObject.CompareTag(targetTag))
        {
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
        // Salva la posizione e rotazione relative iniziali
        Vector3 initialPositionOffset = transform.position - target.transform.position;
        Quaternion initialRotationOffset = Quaternion.Inverse(target.transform.rotation) * transform.rotation;

        float timer = 0f;
        while (timer < attachDuration)
        {
            // Mantieni la posizione e la rotazione relative durante l'attaccamento
            transform.position = target.transform.position + initialPositionOffset;
            transform.rotation = target.transform.rotation * initialRotationOffset;

            timer += Time.deltaTime;
            yield return null;
        }

        // Disattiva l'agganciamento dopo il tempo specificato
        attachCoroutine = null;
    }
}
