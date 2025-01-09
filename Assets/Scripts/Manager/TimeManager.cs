using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    [Header("Quanto rallenta il tempo")]
    public float slowdownFactor = 0.05f;

    [Header("Per quanto tempo")]
    public float slowdownLength = 2f;

    private Coroutine slowMotionCoroutine;

    void Update()
    {
        // Logica per il recupero della velocità del tempo
        if (Time.timeScale < 1f && slowMotionCoroutine == null)
        {
            StartCoroutine(RecoverTimeCoroutine());
        }
    }

    // Coroutine per il rallentamento del tempo
    public IEnumerator DoSlowmotionCoroutine()
    {
        //Debug.Log("Rallento il tempo");
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        // Aspetta per la durata del rallentamento
        float elapsedTime = 0f;
        while (elapsedTime < slowdownLength)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null; // Aspetta un frame
        }

        // Dopo che il tempo è stato rallentato per il periodo specificato, ripristina il tempo normale
        slowMotionCoroutine = null; // Svuota la variabile per permettere altre chiamate a DoSlowmotion
        StartCoroutine(RecoverTimeCoroutine());
    }

    // Coroutine per il recupero del tempo
    private IEnumerator RecoverTimeCoroutine()
    {
        float startTimeScale = Time.timeScale;
        float elapsedTime = 0f;

        // Gradualmente ripristina il tempo fino a 1 (tempo normale)
        while (elapsedTime < slowdownLength)
        {
            Time.timeScale = Mathf.Lerp(startTimeScale, 1f, elapsedTime / slowdownLength);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null; // Aspetta un frame
        }

        // Assicurati che il tempo sia ripristinato esattamente a 1
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f; // Imposta la fixedDeltaTime di default
    }
}
