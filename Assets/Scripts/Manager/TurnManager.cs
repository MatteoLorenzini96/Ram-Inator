using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Tentativi per stelle")]
    public int twoStarsAttempts = 3; // Tentativi massimi per ottenere 2 stelle
    public int threeStarsAttempts = 2; // Tentativi massimi per ottenere 3 stelle

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
         // Calcola la percentuale di oggetti rimanenti
        int remainingObjects = objectsWithCollisionStateChanger.Count;
        float percentage = (float)remainingObjects / initialObjectCount * 100f;

        // Valutazione basata sui tentativi e sugli oggetti rimasti
        if (remainingAttempts >= 0 && remainingAttempts >= twoStarsAttempts)
        {
            // 100% completato
            stars = 1;
        if (remainingAttempts >= threeStarsAttempts)
        {
            // Usa meno tentativi del massimo per 3 stelle
            stars = 3;
            Debug.Log("3 stelle");
        }
        else if (remainingAttempts >= twoStarsAttempts)
        {
            // Usa meno tentativi del massimo per 2 stelle
            stars = 2;
            Debug.Log("2 stelle");
        }
        else
        {
            // Non raggiunge la soglia minima
            stars = 0;
        }
        }

        ActivatePanel();

        // Salva il punteggio nel LevelResultsManager

        LevelResultsManager.Instance.SaveLevelResult(SceneManager.GetActiveScene().buildIndex, stars);
    }

    private void ActivatePanel()
    {
        Debug.Log("Attivo il pannello di fine livello");

        evaluetePanel.SetActive(true);

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

            timeManager.slowdownFactor = 0.01f;
            //timeManager.slowdownLength = 2f;           //Non � necessario aumentare il tempo
            StartCoroutine(timeManager.DoSlowmotionCoroutine());

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
