using UnityEngine;
using EZCameraShake;

public class CollisionStateChanger : MonoBehaviour
{

    private TimeManager timeManager;

    [Header("SoundEffect da riprodurre al cambio di stato")]
    public string soundEffectIntegro = "DestroyGlass"; // Variabile pubblica per modificare il nome del suono dall'Inspector
    public string soundEffectDanneggiato = "DestroyGlass"; // Variabile pubblica per modificare il nome del suono dall'Inspector
    public string soundEffectDestroy = "DestroyGlass"; // Variabile pubblica per modificare il nome del suono dall'Inspector


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
        if (collision.gameObject.CompareTag("Palla")){

            AudioManager.Instance.PlaySFX(soundEffectIntegro); // Usa la variabile per chiamare il metodo

            // Cerca nei figli dell'oggetto con il tag "Palla"
            foreach (Transform child in collision.transform){

                if (child.gameObject.activeSelf){

                    // Controlla se il figlio attivo � nella lista dei tipi distruttivi
                    if (System.Array.Exists(destroyableHeads, head => head == child.name)){

                    // Cambia stato in base alla velocità
                    if (relativeSpeed < lowSpeedThreshold)
                    {
                        ChangeState(ObjectState.Idle);
                    }

                    else if (relativeSpeed >= lowSpeedThreshold && relativeSpeed < highSpeedThreshold)
                    {
                        ChangeState(ObjectState.LowImpact);
                        viteoggetto = viteoggetto - 1;
                        AudioManager.Instance.PlaySFX(soundEffectDanneggiato); // Usa la variabile per chiamare il metodo
                    }

                    else if (relativeSpeed >= highSpeedThreshold)
                    {
                    ChangeState(ObjectState.HighImpact);
                    viteoggetto = viteoggetto - 2;
                    }

                    //Debug.Log($"Velocità relativa: {relativeSpeed}, Nuovo stato: {currentState}, vite: {viteoggetto}");
                    if (replacementPrefab != null && viteoggetto <= 0)
                        {
                            Explode();
                        }
                        return;
                    }
                }
            }
        }
    }


    public void Explode()
    {
        timeManager.DoSlowmotion();  //Applica l'effetto SlowMotion
        CameraShaker.Instance.ShakeOnce(1.5f, 1.5f, .1f, 1f);  //Applica il CameraShake

        Instantiate(replacementPrefab, transform.position, transform.rotation);
        Destroy(gameObject); // Distruggi l'oggetto attuale
        AudioManager.Instance.PlaySFX(soundEffectDestroy); // Usa la variabile per chiamare il metodo
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
}