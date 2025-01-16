using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    [System.Serializable]
    public class EffectEntry
    {
        public string effectName; // Nome dell'effetto
        public GameObject effectPrefab; // Prefab dell'effetto
    }

    public static EffectsManager Instance; // Singleton

    [SerializeField]
    private List<EffectEntry> effectsList = new List<EffectEntry>();

    private Dictionary<string, GameObject> effectsDictionary;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Mantieni l'oggetto al cambio di scena
        }
        else
        {
            Destroy(gameObject);
        }

        // Crea un dizionario per un accesso rapido agli effetti
        effectsDictionary = new Dictionary<string, GameObject>();
        foreach (var entry in effectsList)
        {
            if (!effectsDictionary.ContainsKey(entry.effectName))
            {
                effectsDictionary.Add(entry.effectName, entry.effectPrefab);
            }
            else
            {
                Debug.LogWarning($"Effetto con nome duplicato: {entry.effectName}. Sarà ignorato.");
            }
        }
    }

    /// <summary>
    /// Instanzia un effetto in una posizione specifica con una rotazione specifica e restituisce l'oggetto istanziato.
    /// </summary>
    /// <param name="effectName">Nome dell'effetto.</param>
    /// <param name="position">Posizione dove istanziare l'effetto.</param>
    /// <param name="rotation">Rotazione dell'effetto.</param>
    /// <returns>Il GameObject istanziato.</returns>
    public GameObject SpawnEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        if (effectsDictionary.TryGetValue(effectName, out var effectPrefab))
        {
            return Instantiate(effectPrefab, position, rotation); // Restituisci l'oggetto istanziato
        }
        else
        {
            Debug.LogError($"Effetto non trovato: {effectName}. Assicurati che il nome sia corretto e che l'effetto sia aggiunto alla lista.");
            return null; // Restituisci null se l'effetto non è trovato
        }
    }

    /// <summary>
    /// Instanzia un effetto nella posizione e rotazione predefinita (identity) e restituisce l'oggetto istanziato.
    /// </summary>
    /// <param name="effectName">Nome dell'effetto.</param>
    /// <param name="position">Posizione dove istanziare l'effetto.</param>
    /// <returns>Il GameObject istanziato.</returns>
    public GameObject SpawnEffect(string effectName, Vector3 position)
    {
        return SpawnEffect(effectName, position, Quaternion.identity); // Chiama la versione con rotazione predefinita
    }
}
