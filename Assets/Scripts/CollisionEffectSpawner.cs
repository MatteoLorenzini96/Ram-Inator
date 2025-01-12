using System.Collections.Generic;
using UnityEngine;

public class CollisionEffectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class TagEffect
    {
        public string tag; // Tag dell'oggetto
        public string effectName; // Nome dell'effetto (deve corrispondere a quello registrato in EffectsManager)
    }

    [SerializeField]
    private List<TagEffect> tagEffectList = new List<TagEffect>();

    private Dictionary<string, string> tagEffectDictionary;

    private void Awake()
    {
        // Crea un dizionario per accesso rapido agli effetti in base al tag
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
        if (tagEffectDictionary.TryGetValue(otherObject.tag, out var effectName))
        {
                //Debug.Log("Il Tag ha un effetto assegnato");
            if (EffectsManager.Instance != null)
            {
                // Calcola la rotazione usando la direzione opposta all'impatto
                Vector3 invertedNormal = -impactNormal;
                Quaternion rotation = Quaternion.LookRotation(invertedNormal);

                    //l'effetto viene spawnato in direzione dell'impatto
                //Quaternion rotation = Quaternion.LookRotation(impactNormal);

                EffectsManager.Instance.SpawnEffect(effectName, impactPoint, rotation);
                    //Debug.Log("Effetto spawnato correttamente");
            }
            else
            {
                Debug.LogError("EffectsManager non trovato nella scena. Assicurati che sia presente.");
            }
        }
    }
}
