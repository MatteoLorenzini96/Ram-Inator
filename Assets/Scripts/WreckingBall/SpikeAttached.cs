using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpikeAttached : MonoBehaviour
{
    [SerializeField] private Transform parentObject;
    private int activeChildIndex = 1;
    private WreckingBallDrag wreckingBallDrag;

    public string targetTag = "TargetTag";
    public float attachDuration = 5f;
    private Transform activeChild;
    private bool isScriptActive = false;
    private Coroutine attachCoroutine;
    public bool isSpikeAttached = false;
    public event Action OnDetach;

    private HashSet<GameObject> temporarilyBlockedObjects = new HashSet<GameObject>();
    public float reattachCooldown = 3f;

    [SerializeField] private string noCollisionLayerName = "NoCollision"; // Nome del layer NoCollision

    private void Start()
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent Object non assegnato! Assegna un parent nel campo 'Parent Object' nell'Inspector.");
            return;
        }

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
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Controlla se l'oggetto è nel layer NoCollision
        if (gameObject.layer == LayerMask.NameToLayer(noCollisionLayerName))
        {
            Debug.Log("Oggetto nel layer NoCollision, nessuna azione eseguita.");
            return;
        }

        if (isScriptActive && collision.gameObject.CompareTag(targetTag) && wreckingBallDrag.isSwinging)
        {
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
        }
    }

    private IEnumerator AttachToTarget(GameObject target)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 initialPositionOffset = transform.position - target.transform.position;
        Quaternion initialRotationOffset = Quaternion.Inverse(target.transform.rotation) * transform.rotation;

        float timer = 0f;
        while (timer < attachDuration)
        {
            isSpikeAttached = true;

            if (rb != null)
            {
                rb.isKinematic = true;
            }

            transform.position = target.transform.position + initialPositionOffset;
            transform.rotation = target.transform.rotation * initialRotationOffset;

            timer += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        attachCoroutine = null;
        isSpikeAttached = false;

        TemporarilyBlockObject(target);

        OnDetach?.Invoke();
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
        }
    }
}
