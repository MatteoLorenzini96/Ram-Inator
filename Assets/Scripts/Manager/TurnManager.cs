using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using EZCameraShake;

public class TurnManager : MonoBehaviour
{
    private TimeManager timeManager;
    
    [Header("Numero di tentativi")]
    public int maxAttempts = 5; // Numero massimo di tentativi impostabile dall'inspector
    public TextMeshProUGUI attemptsText;

    [Header("Valutazione")]
    public GameObject evaluetePanel;
    public List<GameObject> starsUI; // Lista di stelle impostabile dall'Inspector
    public int stars = 0;

    [Header("Oggetti per stelle non attive")]
    public List<GameObject> inactiveStars; // Lista di oggetti figli alternativi per stelle non attive

    [Header("Tentativi per stelle")]
    public int twoStarsAttempts = 3; // Tentativi massimi per ottenere 2 stelle
    public int threeStarsAttempts = 2; // Tentativi massimi per ottenere 3 stelle

    [Header("Testo per valutazione")]
    //public GameObject defaultChildObject; // Oggetto figlio predefinito
    //public GameObject alternativeChildObject; // Oggetto figlio alternativo per 0 stelle
    public TextMeshProUGUI evaluateText;

    private Transform cameraParent; // Variabile per il parent della telecamera
    private List<GameObject> objectsWithCollisionStateChanger;
    private int initialObjectCount;
    private int remainingAttempts;
    private Vector3 lastHitPosition; // Posizione dell'ultimo oggetto colpito
    private bool isFocusDone = false;

    void Start()
    {
        remainingAttempts = maxAttempts;

        // Aggiorna il testo UI con il numero di tentativi
        UpdateAttemptsText();

        // Cerca il parent della telecamera per nome
        GameObject cameraHolderObject = GameObject.Find("CameraHolder");
        if (cameraHolderObject != null)
        {
            cameraParent = cameraHolderObject.transform;
        }
        else
        {
            Debug.LogWarning("Il parent della telecamera con nome 'CameraHolder' non � stato trovato!");
        }

        // Cerca il TimeManager
        if (timeManager == null)
        {
            timeManager = FindFirstObjectByType<TimeManager>();
            if (timeManager == null)
            {
                Debug.LogError("Non � stato trovato un oggetto con TimeManager nella scena!");
            }
        }

        // Trova tutti gli oggetti con lo script "CollisionStateChanger" e crea l'indice
        objectsWithCollisionStateChanger = new List<GameObject>();
#pragma warning disable CS0618 // Il tipo o il membro � obsoleto
        foreach (var obj in FindObjectsOfType<MonoBehaviour>())
        {
            if (obj.GetComponent<CollisionStateChanger>() != null)
            {
                objectsWithCollisionStateChanger.Add(obj.gameObject);
            }
        }
#pragma warning restore CS0618 // Il tipo o il membro � obsoleto

        // Salva il numero iniziale di oggetti
        initialObjectCount = objectsWithCollisionStateChanger.Count;
    }

    private void Update()
    {
        {
            //Debug.Log($"Numero di oggetti nella lista: {objectsWithCollisionStateChanger.Count}");

            if (objectsWithCollisionStateChanger.Count > 0)
            {
                // Rimuove gli oggetti distrutti
                objectsWithCollisionStateChanger.RemoveAll(obj => obj == null);

                // Aggiorna la posizione dell'ultimo oggetto
                if (objectsWithCollisionStateChanger.Count > 0)
                {
                    lastHitPosition = objectsWithCollisionStateChanger[0].transform.position;
                    //Debug.Log($"Ultima posizione aggiornata: {lastHitPosition}");
                }
            }

            if (objectsWithCollisionStateChanger.Count == 0)
            {
                if (lastHitPosition != Vector3.zero && isFocusDone == false)
                {
                    FocusOnLastPosition();
                    //Debug.Log("Sposto la camera sulla posizione salvata.");
                }
                else
                {
                    //Debug.LogWarning("Lista vuota e nessuna posizione salvata.");
                }
            }

            UpdateAttemptsText();
        }
    }

    private void UpdateAttemptsText()
    {
        if (attemptsText != null && attemptsText.text != "Lanci: " + remainingAttempts)
        {
            attemptsText.text = "Lanci: " + remainingAttempts;
        }
    }


    public void OnReset()
    {
        // Sottrai un tentativo e aggiorna il testo
        remainingAttempts--;
        UpdateAttemptsText();

        // Se i tentativi sono finiti, valuta gli oggetti rimasti
        if (remainingAttempts <= 0)
        {
            EvaluateObjects();
        }
    }

    private void EvaluateObjects()
    {        
       int remainingObjects = objectsWithCollisionStateChanger.Count;
    float percentage = (float)remainingObjects / initialObjectCount * 100f;

    // Valutazione basata sui tentativi e sugli oggetti rimasti
    if (percentage == 0f)
    {
        // Completato al 100% (tutti gli oggetti distrutti)
        if (remainingAttempts >= threeStarsAttempts)
        {
            // Usa meno tentativi del massimo per 3 stelle
            stars = 3;
            //Debug.Log("3 stelle");
        }
        else if (remainingAttempts >= twoStarsAttempts)
        {
            // Usa meno tentativi del massimo per 2 stelle
            stars = 2;
            //Debug.Log("2 stelle");
        }
        else if (remainingAttempts < twoStarsAttempts )
        {
            // Usa più tentativi di quelli previsti per 2 stelle ma meno del massimo
            stars = 1;
            //Debug.Log("1 stella - Tentativi superiori a quelli per 2 stelle, ma inferiori al massimo");
        }
        else
        {
            // Usa più tentativi del massimo
            stars = 0;
            //Debug.Log("1 stelle - Tentativi superiori al massimo");
        }
    }
    else
    {
        // Se non hai completato il livello (ci sono ancora oggetti da distruggere)
        stars = 0;  // Completamento parziale, quindi solo 1 stella
        //Debug.Log("0 stelle - Completamento parziale");
    }
    

    ActivatePanel();

    // Salva il punteggio nel LevelResultsManager
    LevelResultsManager.Instance.SaveLevelResult(SceneManager.GetActiveScene().buildIndex, stars);
}


    private void ActivatePanel()
    {
        //Debug.Log("Attivo il pannello di fine livello");

        evaluetePanel.SetActive(true);

        // Disattiva tutti gli oggetti figli delle stelle non attive inizialmente
        foreach (var inactiveStar in inactiveStars)
        {
            if (inactiveStar != null)
                inactiveStar.SetActive(false);
        }

        // Calcola il numero di stelle non ottenute
        int inactiveStarCount = Mathf.Clamp(3 - stars, 0, inactiveStars.Count);

        if (stars == 0)
        {
            evaluateText.text = "Ritenta!";

            //Debug.Log("Text 0 stelle.");

            /*if (defaultChildObject != null)
                 defaultChildObject.SetActive(false); // Disattiva l'oggetto predefinito

             if (alternativeChildObject != null)
                 alternativeChildObject.SetActive(true); // Attiva l'oggetto alternativo*/
        }
        if (stars == 1)
        {
            evaluateText.text = "Puoi fare di meglio!";

            //Debug.Log("Text 1 stella.");

            /*if (defaultChildObject != null)
            defaultChildObject.SetActive(true); // Attiva l'oggetto predefinito
            
            if (alternativeChildObject != null)
            alternativeChildObject.SetActive(false); // Disattiva l'oggetto alternativo*/
        }
        if (stars == 2)
        {
            evaluateText.text = "Ci sei quasi!";

            //Debug.Log("Text 2 stelle.");

            /*if (defaultChildObject != null)
            defaultChildObject.SetActive(true); // Attiva l'oggetto predefinito
            
            if (alternativeChildObject != null)
            alternativeChildObject.SetActive(false); // Disattiva l'oggetto alternativo*/
        }
        if (stars == 3)
        {
            evaluateText.text = "Complimenti!";

            //Debug.Log("Text 3 stelle.");

            /*if (defaultChildObject != null)
            defaultChildObject.SetActive(true); // Attiva l'oggetto predefinito
            
            if (alternativeChildObject != null)
            alternativeChildObject.SetActive(false); // Disattiva l'oggetto alternativo*/
        }
        // Attiva gli oggetti figli per le stelle non ottenute
        for (int i = 0; i < inactiveStarCount; i++)
        {
            if (inactiveStars[i] != null)
                inactiveStars[i].SetActive(true);
        }

        // Attiva le stelle UI in base al numero di stelle calcolato
        ActivateStarsUI(stars);
    }

    private void ActivateStarsUI(int stars)
    {// Assicurati che la lista non sia vuota
        if (starsUI == null || starsUI.Count == 0)
        {
            Debug.LogWarning("La lista delle stelle UI non è configurata!");
            return;
        }

        // Disattiva tutte le stelle
        foreach (var star in starsUI)
        {
            star.SetActive(false);
        }

        // Attiva il numero di stelle calcolato
        for (int i = 0; i < stars && i < starsUI.Count; i++)
        {
            starsUI[i].SetActive(true);
        }
    }

    private void FocusOnLastPosition()
    {
        if (cameraParent != null)
        {
            //Debug.Log($"Muovo la telecamera verso la posizione salvata: {lastHitPosition}");

            // Muove il parent della telecamera verso l'ultima posizione salvata
            cameraParent.position = new Vector3(
                lastHitPosition.x,
                lastHitPosition.y,
                lastHitPosition.z - 8f // Aggiusta la profondit� se necessario
            );

            // (Facoltativo) Riorienta la telecamera verso la posizione
            Camera.main.transform.LookAt(lastHitPosition);

            //Debug.Log("Telecamera spostata sulla posizione salvata.");

            // Usa la coroutine per attivare il controllo dopo un tempo reale
            StartCoroutine(EvaluateObjectsAfterDelay(2f));
            isFocusDone = true;

            timeManager.slowdownFactor = 0.005f;
            timeManager.slowdownLength = 4f;           //Non � necessario aumentare il tempo
            StartCoroutine(timeManager.DoSlowmotionCoroutine());
            CameraShaker.Instance.ShakeOnce(5f, 5f, .2f, 2f);  //Applica il CameraShake

        }
        else
        {
            Debug.LogWarning("cameraParent � null, impossibile spostare la telecamera.");
        }
    }

    private IEnumerator EvaluateObjectsAfterDelay(float delay)
    {
        // Aspetta un tempo reale, ignorando il Time.timeScale
        yield return new WaitForSecondsRealtime(delay);

        // Attiva il controllo
        EvaluateObjects();
    }
}
