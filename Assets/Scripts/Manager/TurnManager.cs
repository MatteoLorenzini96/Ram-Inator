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
            Debug.LogWarning("Il parent della telecamera con nome 'CameraHolder' non è stato trovato!");
        }

        // Cerca il TimeManager
        if (timeManager == null)
        {
            timeManager = FindFirstObjectByType<TimeManager>();
            if (timeManager == null)
            {
                Debug.LogError("Non è stato trovato un oggetto con TimeManager nella scena!");
            }
        }

        // Trova tutti gli oggetti con lo script "CollisionStateChanger" e crea l'indice
        objectsWithCollisionStateChanger = new List<GameObject>();
#pragma warning disable CS0618 // Il tipo o il membro è obsoleto
        foreach (var obj in FindObjectsOfType<MonoBehaviour>())
        {
            if (obj.GetComponent<CollisionStateChanger>() != null)
            {
                objectsWithCollisionStateChanger.Add(obj.gameObject);
            }
        }
#pragma warning restore CS0618 // Il tipo o il membro è obsoleto

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
        if (attemptsText != null)
        {
            attemptsText.text = "Tentativi: " + remainingAttempts;
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


        if (percentage > 50f)
        {
            Debug.Log("fai schifo");
            stars = 0;
        }
        else if (percentage <= 50f && percentage > 25f)
        {
            Debug.Log("meh");
            stars = 1;
        }
        else if (percentage <= 25f && percentage > 0f)
        {
            Debug.Log("Skill Issue");
            stars = 2;
        }
        else if (percentage == 0f)
        {
            Debug.Log("GG EZ");
            stars = 3;
        }

        ActivatePanel();
        // Salva il punteggio nel GameManager
        //GameManager.Instance.SetLevelStars(SceneManager.GetActiveScene().buildIndex, stars);
    }

    private void ActivatePanel()
    {
        Debug.Log("Attivo il pannello di fine livello");

        evaluetePanel.SetActive(true);

        // Attiva le stelle UI in base al numero di stelle calcolato
        ActivateStarsUI(stars);
    }

    private void ActivateStarsUI(int stars)
    {
        // Assicurati che la lista non sia vuota
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
                lastHitPosition.z - 8f // Aggiusta la profondità se necessario
            );

            // (Facoltativo) Riorienta la telecamera verso la posizione
            Camera.main.transform.LookAt(lastHitPosition);

            //Debug.Log("Telecamera spostata sulla posizione salvata.");

            // Usa la coroutine per attivare il controllo dopo un tempo reale
            StartCoroutine(EvaluateObjectsAfterDelay(2f));
            isFocusDone = true;

            timeManager.slowdownFactor = 0.01f;
            //timeManager.slowdownLength = 4f;           //Non è necessario aumentare il tempo
            timeManager.DoSlowmotion();  //Applica l'effetto SlowMotion

        }
        else
        {
            Debug.LogWarning("cameraParent è null, impossibile spostare la telecamera.");
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
