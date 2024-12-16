using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public float timerDuration = 10f;
    public TextMeshProUGUI timerText;

    private TimeManager timeManager;
    private Transform cameraParent; // Variabile per il parent della telecamera
    private List<GameObject> objectsWithCollisionStateChanger;
    private int initialObjectCount;
    private float timer;
    private bool timerRunning = false;
    private bool isFirstTimerStarted = false; // Nuova variabile per controllare il primo avvio
    private Vector3 lastHitPosition; // Posizione dell'ultimo oggetto colpito

    void Start()
    {
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
        foreach (var obj in FindObjectsOfType<MonoBehaviour>())
        {
            if (obj.GetComponent<CollisionStateChanger>() != null)
            {
                objectsWithCollisionStateChanger.Add(obj.gameObject);
            }
        }

        // Salva il numero iniziale di oggetti
        initialObjectCount = objectsWithCollisionStateChanger.Count;

        // Cerca WreckingBallDrag e si iscrive al suo evento OnRelease
        WreckingBallDrag wreckingBallDrag = FindFirstObjectByType<WreckingBallDrag>();
        if (wreckingBallDrag != null)
        {
            wreckingBallDrag.OnRelease += OnStopDragging;
        }

        // Assicurati che il testo del timer sia inizialmente nascosto
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    void Update()
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
            if (lastHitPosition != Vector3.zero)
            {
                FocusOnLastPosition();
                //Debug.Log("Sposto la camera sulla posizione salvata.");
            }
            else
            {
                Debug.LogWarning("Lista vuota e nessuna posizione salvata.");
            }
        }

        TimerManager();
    }

    private void TimerManager()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(timer).ToString();

                if (timer <= timerDuration / 2f)
                {
                    timerText.gameObject.SetActive(true);
                }
            }

            if (timer <= 0f)
            {
                timerRunning = false;
                EvaluateObjects();
            }
        }
    }

    private void OnStopDragging(Vector3 releaseVelocity)
    {
        if (isFirstTimerStarted)
        {
            return;
        }

        timer = timerDuration;
        timerRunning = true;
        isFirstTimerStarted = true;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
            timerText.text = timerDuration.ToString();
        }
    }

    private void EvaluateObjects()
    {
        int remainingObjects = objectsWithCollisionStateChanger.Count;
        float percentage = (float)remainingObjects / initialObjectCount * 100f;

        if (percentage > 50f)
        {
            Debug.Log("fai schifo");
        }
        else if (percentage <= 50f && percentage > 25f)
        {
            Debug.Log("meh");
        }
        else if (percentage <= 25f && percentage > 0f)
        {
            Debug.Log("Skill Issue");
        }
        else if (percentage == 0f)
        {
            Debug.Log("GG EZ");
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

            timeManager.slowdownFactor = 0.01f;
            //timeManager.slowdownLength = 4f;           //Non è necessario aumentare il tempo
            timeManager.DoSlowmotion();  //Applica l'effetto SlowMotion

        }
        else
        {
            Debug.LogWarning("cameraParent è null, impossibile spostare la telecamera.");
        }
    }
}
