using UnityEngine;
using EZCameraShake;
using System.Collections.Generic;

public class CollisionStateChanger : MonoBehaviour
{
    private TimeManager timeManager;
    private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();

    [Header("SoundEffect da riprodurre al cambio di stato")]
    public string soundEffectIntegro = "DestroyGlass"; // Variabile pubblica per modificare il nome del suono dall'Inspector
    public string soundEffectDanneggiato = "DestroyGlass"; // Variabile pubblica per modificare il nome del suono dall'Inspector
    public string soundEffectDestroy = "DestroyGlass"; // Variabile pubblica per modificare il nome del suono dall'Inspector
    public float soundCooldown = 0.5f; // Tempo minimo tra due riproduzioni di qualsiasi suono

    [Header("Soglie di velocità")]
    // Soglie di velocità per cambiare stato
    public float lowSpeedThreshold = 5f;  // Velocità bassa
    public float highSpeedThreshold = 10f; // Velocità alta

    [Header("Numero di Vite")]
    // Numero di vite dell'oggetto
    public float viteoggetto = 2f;

    [Header("Prefab di Sostituzione")]
    public float minFragmented = 0f;
    public GameObject lowSpeedPrefab; // Prefab da utilizzare per velocità bassa
    public float medFragmented = 10f;
    public GameObject mediumSpeedPrefab; // Prefab da utilizzare per velocità media
    public float maxFragmented = 15f;
    public GameObject highSpeedPrefab; // Prefab da utilizzare per velocità alta
    private GameObject selectedPrefab;

    [Header("Tipi di Head Distruttivi")]
    public string[] destroyableHeads; // Nomi dei "Head" che possono essere distrutti

    public Vector3 lastImpactPoint { get; private set; } // Punto di impatto registrato

    // Stato attuale dell'oggetto
    public enum ObjectState
    {
        Idle,
        LowImpact,
        HighImpact
    }

    public ObjectState currentState = ObjectState.Idle;

    private void Awake()
    {
        if (timeManager == null)
        {
            timeManager = FindFirstObjectByType<TimeManager>(); // Cerca il primo oggetto di tipo TimeManager nella scena
            if (timeManager == null)
            {
                Debug.LogError("Non è stato trovato un oggetto con TimeManager nella scena!"); // Messaggio di errore se non trovato
            }
        }
    }

    // Metodo chiamato al momento della collisione
    private void OnCollisionEnter(Collision collision)
    {
        // Calcolo della velocità relativa
        float relativeSpeed = collision.relativeVelocity.magnitude;

        // Controlla se l'oggetto che ha colpito ha il tag corretto
        if (collision.gameObject.CompareTag("Palla"))
        {
            lastImpactPoint = collision.GetContact(0).point; // Registra il punto di impatto
            PlaySoundWithCooldown(soundEffectIntegro);

            foreach (Transform child in collision.transform)
            {
                if (child.gameObject.activeSelf &&
                    System.Array.Exists(destroyableHeads, head => head == child.name))
                {
                    if (relativeSpeed < lowSpeedThreshold)
                    {
                        ChangeState(ObjectState.Idle);
                    }
                    else if (relativeSpeed >= lowSpeedThreshold && relativeSpeed < highSpeedThreshold)
                    {
                        ChangeState(ObjectState.LowImpact);
                        viteoggetto -= 1; // Riduci di 1 il numero di vite
                        PlaySoundWithCooldown(soundEffectDanneggiato);
                        //Debug.Log("Colpito con velocità bassa, tolgo 1 vita");
                    }
                    else if (relativeSpeed >= highSpeedThreshold)
                    {
                        ChangeState(ObjectState.HighImpact);
                        viteoggetto -= 2; // Riduci di 2 il numero di vite
                        PlaySoundWithCooldown(soundEffectDestroy);
                        //Debug.Log("Colpito con velocità alta, tolgo 2 vite");
                    }

                    if (viteoggetto <= 0)
                    {
                        //Debug.Log("Vita finita, attivo l'esplosione");
                        Explode(relativeSpeed); // Chiama il metodo Explode e passa la velocità d'impatto
                    }
                    return;
                }
            }
        }
    }

    // Metodo chiamato per gestire l'esplosione e la sostituzione dell'oggetto
    public void Explode(float relativeSpeed)
    {
        StartCoroutine(timeManager.DoSlowmotionCoroutine()); // Avvia la modalità slow-motion
        CameraShaker.Instance.ShakeOnce(2.5f, 2.5f, .1f, 1f); // Applica il CameraShake
        VibratePhone(); // Esegui la vibrazione del telefono

        // Seleziona il prefab corretto in base alla velocità d'impatto
        if (relativeSpeed >= minFragmented && relativeSpeed < medFragmented)
        {
            selectedPrefab = lowSpeedPrefab;
            //Debug.Log("Velocità bassa");
        }
        else if (relativeSpeed >= medFragmented && relativeSpeed < maxFragmented)
        {
            selectedPrefab = mediumSpeedPrefab;
            //Debug.Log("Velocità media");
        }
        else if (relativeSpeed >= maxFragmented)
        {
            selectedPrefab = highSpeedPrefab;
            //Debug.Log("Velocità alta");
        }

        if (selectedPrefab != null)
        {
            GameObject replacement = Instantiate(selectedPrefab, transform.position, transform.rotation); // Istanzia il prefab selezionato
            replacement.transform.localScale = transform.localScale; // Mantieni la scala originale

            //Debug.Log("Spawno frammenti in base alla velocità");

            // Passa il punto di impatto al prefab sostituto, se contiene uno script ParentCollisionManager
            ParentCollisionManager pcm = replacement.GetComponent<ParentCollisionManager>();
            if (pcm != null)
            {
                pcm.SetImpactPoint(lastImpactPoint);
            }

            Destroy(gameObject); // Distruggi l'oggetto attuale
            AudioManager.Instance.PlaySFX(soundEffectDestroy); // Usa la variabile per chiamare il metodo
        }
    }

    // Metodo per far vibrare il telefono
    private void VibratePhone()
    {
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    // Metodo per cambiare lo stato
    public void ChangeState(ObjectState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            // Puoi aggiungere qui il codice per gestire i cambiamenti di stato
            // Debug.Log($"Lo stato è cambiato a: {currentState} e ha vite: {viteoggetto}");
        }
    }

    // Metodo per riprodurre un suono con cooldown
    private void PlaySoundWithCooldown(string soundEffect)
    {
        float currentTime = Time.time;

        // Verifica se il suono ha già un tempo di cooldown registrato
        if (soundCooldowns.ContainsKey(soundEffect))
        {
            // Controlla se il cooldown è scaduto
            if (currentTime - soundCooldowns[soundEffect] < soundCooldown)
            {
                return; // Ignora se il cooldown non è scaduto
            }
        }

        // Aggiorna il tempo dell'ultima riproduzione del suono
        soundCooldowns[soundEffect] = currentTime;

        // Riproduce il suono
        AudioManager.Instance.PlaySFX(soundEffect);
    }
}
