using System.Collections.Generic;
using UnityEngine;

public class MetalCollisionEffectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class TagEffect
    {
        public string tag; // Tag dell'oggetto
        public string effectName; // Nome dell'effetto (deve corrispondere a quello registrato in EffectsManager)
    }

    [System.Serializable]
    public class PallaEffect
    {
        public string headName; // Nome del child (NormalHead, SpikeHead, ImpactHead)
        public string effectName; // Nome dell'effetto associato al child
    }

    [SerializeField]
    private List<TagEffect> tagEffectList = new List<TagEffect>(); // Lista generale degli effetti per i tag

    [SerializeField]
    private List<PallaEffect> pallaEffectList = new List<PallaEffect>(); // Lista degli effetti per il tag "Palla" basato sul child attivo

    private Dictionary<string, string> tagEffectDictionary; // Dizionario per tag generali
    private Dictionary<string, string> pallaEffectDictionary; // Dizionario per effetti legati ai children di "Palla"

    private void Awake()
    {
        // Crea un dizionario per accesso rapido agli effetti in base al tag generico
        tagEffectDictionary = new Dictionary<string, string>();
        foreach (var entry in tagEffectList)
        {
            if (!tagEffectDictionary.ContainsKey(entry.tag))
            {
                tagEffectDictionary.Add(entry.tag, entry.effectName);
            }
            else
            {
                Debug.LogWarning($"Tag duplicato nella lista: {entry.tag}. Sarà ignorato.");
            }
        }

        // Crea un dizionario per accesso rapido agli effetti in base al child di "Palla"
        pallaEffectDictionary = new Dictionary<string, string>();
        foreach (var entry in pallaEffectList)
        {
            if (!pallaEffectDictionary.ContainsKey(entry.headName))
            {
                pallaEffectDictionary.Add(entry.headName, entry.effectName);
            }
            else
            {
                Debug.LogWarning($"Child duplicato nella lista Palla: {entry.headName}. Sarà ignorato.");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleEffectSpawn(collision.gameObject, collision.contacts[0].point, collision.contacts[0].normal);
        //Debug.Log("Collisione avvenuta, controllo il tag");
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleEffectSpawn(other.gameObject, other.ClosestPoint(transform.position), other.transform.forward);
    }

    private void HandleEffectSpawn(GameObject otherObject, Vector3 impactPoint, Vector3 impactNormal)
    {
        if (otherObject.CompareTag("Palla"))
        {
            // Se l'oggetto ha il tag "Palla", controlla quale child è attivo
            string activeHeadName = GetActiveHeadName(otherObject);

            if (pallaEffectDictionary.TryGetValue(activeHeadName, out var effectName))
            {
                // Se c'è un effetto associato al child attivo, spawnalo
                SpawnEffect(effectName, impactPoint, impactNormal);
            }
            else
            {
                Debug.LogWarning($"Nessun effetto trovato per {activeHeadName} nella lista PallaEffect.");
            }
        }
        else if (tagEffectDictionary.TryGetValue(otherObject.tag, out var effectName))
        {
            // Se l'oggetto non è una "Palla", usa la lista generale
            SpawnEffect(effectName, impactPoint, impactNormal);
        }
    }

    private string GetActiveHeadName(GameObject pallaObject)
    {
        // Restituisce il nome del child attivo (NormalHead, SpikeHead, ImpactHead)
        // Qui dovresti adattare il codice in base a come i tuoi child vengono attivati nella scena
        if (pallaObject.transform.Find("NormalHead")?.gameObject.activeInHierarchy == true)
        {
            return "NormalHead";
        }
        if (pallaObject.transform.Find("SpikeHead")?.gameObject.activeInHierarchy == true)
        {
            return "SpikeHead";
        }
        if (pallaObject.transform.Find("ImpactHead")?.gameObject.activeInHierarchy == true)
        {
            return "ImpactHead";
        }

        return string.Empty; // Se nessun child è attivo, restituisce stringa vuota
    }

    private void SpawnEffect(string effectName, Vector3 impactPoint, Vector3 impactNormal)
    {
        if (EffectsManager.Instance != null)
        {
            // Calcola la rotazione usando la direzione opposta all'impatto
            Vector3 invertedNormal = -impactNormal;
            Quaternion rotation = Quaternion.LookRotation(invertedNormal);

            // L'effetto viene spawnato in direzione dell'impatto
            EffectsManager.Instance.SpawnEffect(effectName, impactPoint, rotation);
            //Debug.Log("Effetto spawnato correttamente");
        }
        else
        {
            Debug.LogError("EffectsManager non trovato nella scena. Assicurati che sia presente.");
        }
    }
}
