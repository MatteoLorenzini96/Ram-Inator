using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectMapping
{
    public string headName; // Nome della testa (NormalHead, SpikeHead, ImpactHead)
    public string effectName; // Nome dell'effetto da instanziare
}

public class ObjectStateEffectSpawner : MonoBehaviour
{
    [Header("Effetti per stato")]
    public List<EffectMapping> idleEffects = new List<EffectMapping>(); // Effetti per vite piene
    public List<EffectMapping> halfLifeEffects = new List<EffectMapping>(); // Effetti per metà vite
    public List<EffectMapping> explodeEffects = new List<EffectMapping>(); // Effetti per vite esaurite

    private CollisionStateChanger collisionStateChanger; // Riferimento allo script CollisionStateChanger
    private GameObject objectEffectsHolder; // Oggetto che contiene tutti gli effetti instanziati

    private void Start()
    {
        // Trova o crea l'oggetto ObjectEffectsHolder nella gerarchia
        objectEffectsHolder = GameObject.Find("ObjectEffectsHolder");
        if (objectEffectsHolder == null)
        {
            objectEffectsHolder = new GameObject("ObjectEffectsHolder");
        }

        // Trova il componente CollisionStateChanger associato a questo oggetto
        collisionStateChanger = GetComponent<CollisionStateChanger>();
        if (collisionStateChanger == null)
        {
            Debug.LogError("Non è stato trovato lo script CollisionStateChanger!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se l'oggetto in collisione ha il tag "Palla"
        if (collision.gameObject.CompareTag("Palla"))
        {
            GameObject activeHead = GetActiveHead(collision.gameObject);

            if (activeHead != null && collisionStateChanger != null)
            {
                // Determina quale effetto instanziare in base alle vite attuali
                if (collisionStateChanger.viteoggetto >= 2)
                {
                    InstantiateEffectForState(activeHead, idleEffects);
                }
                else if (collisionStateChanger.viteoggetto == 1)
                {
                    InstantiateEffectForState(activeHead, halfLifeEffects);
                }
                else if (collisionStateChanger.viteoggetto <= 0)
                {
                    InstantiateEffectForState(activeHead, explodeEffects);
                }
            }
        }
    }

    // Ottiene il child attivo tra NormalHead, SpikeHead o ImpactHead dell'oggetto in collisione
    private GameObject GetActiveHead(GameObject ball)
    {
        foreach (Transform child in ball.transform)
        {
            if (child.gameObject.activeSelf &&
                (child.name == "NormalHead" || child.name == "SpikeHead" || child.name == "ImpactHead"))
            {
                return child.gameObject;
            }
        }
        return null;
    }

    // Instanzia l'effetto corrispondente in base al tipo di head attivo e alla lista di mapping fornita
    private void InstantiateEffectForState(GameObject activeHead, List<EffectMapping> effectMappings)
    {
        if (activeHead != null)
        {
            // Trova il mapping corrispondente al nome della testa attiva
            EffectMapping mapping = effectMappings.Find(e => e.headName == activeHead.name);

            if (mapping != null && !string.IsNullOrEmpty(mapping.effectName))
            {
                // Calcola la posizione per lo spawn
                Vector3 direction = (transform.position - collisionStateChanger.lastImpactPoint).normalized;
                Vector3 spawnPosition = collisionStateChanger.lastImpactPoint + direction * 0.5f;

                // Messaggi di debug per informazioni sulla collisione, la testa attiva e l'effetto istanziato
              //Debug.Log($"Collisione rilevata con oggetto: {activeHead.transform.parent.name}");
              //Debug.Log($"Testa attiva: {activeHead.name}");
              //Debug.Log($"Vite attuali dell'oggetto: {collisionStateChanger.viteoggetto}. Istanzio l'effetto: {mapping.effectName}");

                // Usa EffectsManager per spawnare l'effetto
                GameObject spawnedEffect = EffectsManager.Instance.SpawnEffect(mapping.effectName, spawnPosition);
                if (spawnedEffect != null)
                {
                    spawnedEffect.transform.SetParent(objectEffectsHolder.transform);
                }

            }
            else
            {
                Debug.LogWarning($"Effetto non trovato per la testa: {activeHead.name}");
            }
        }
        else
        {
            Debug.LogWarning("ActiveHead non trovato!");
        }
    }
}
