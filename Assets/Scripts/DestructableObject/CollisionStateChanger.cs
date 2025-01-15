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
    public GameObject replacementPrefab;

    [Header("Tipi di Head Distruttivi")]
    public string[] destroyableHeads;

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
            timeManager = FindFirstObjectByType<TimeManager>();
            if (timeManager == null)
            {
                Debug.LogError("Non è stato trovato un oggetto con TimeManager nella scena!");
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
                        viteoggetto -= 1;
                        PlaySoundWithCooldown(soundEffectDanneggiato);
                    }
                    else if (relativeSpeed >= highSpeedThreshold)
                    {
                        ChangeState(ObjectState.HighImpact);
                        viteoggetto -= 2;
                        PlaySoundWithCooldown(soundEffectDestroy);
                    }

                    if (replacementPrefab != null && viteoggetto <= 0)
                    {
                        Explode();
                    }
                    return;
                }
            }
        }

    }

    public void Explode()
    {
        StartCoroutine(timeManager.DoSlowmotionCoroutine());
        CameraShaker.Instance.ShakeOnce(2.5f, 2.5f, .1f, 1f);  //Applica il CameraShake

        // Esegui la vibrazione del telefono
        VibratePhone();

        // Calcola la nuova scala proporzionale
        float scaleFactor = 0.5f; // La proporzione tra la scala originale (0.7) e quella del prefab (0.35)
        Vector3 newScale = transform.localScale * scaleFactor;

        // Crea il prefab sostituto e passa il punto di impatto
        GameObject replacement = Instantiate(replacementPrefab, transform.position, transform.rotation);
        replacement.transform.localScale = newScale;

        // Passa il punto di impatto al prefab sostituto, se contiene uno script ParentCollisionManager
        ParentCollisionManager pcm = replacement.GetComponent<ParentCollisionManager>();
        if (pcm != null)
        {
            pcm.SetImpactPoint(lastImpactPoint);
        }

        Destroy(gameObject); // Distruggi l'oggetto attuale
        AudioManager.Instance.PlaySFX(soundEffectDestroy); // Usa la variabile per chiamare il metodo
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
            //Debug.Log($"Lo stato è cambiato a: {currentState} e ha vite: {viteoggetto}");
        }
    }

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