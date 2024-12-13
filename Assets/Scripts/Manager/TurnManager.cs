using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necessario per gestire TextMeshPro

public class TurnManager : MonoBehaviour
{
    // Variabili pubbliche modificabili dall'Inspector
    public float timerDuration = 10f;
    public TextMeshProUGUI timerText;

    // Variabili private
    private List<GameObject> objectsWithCollisionStateChanger;
    private int initialObjectCount;
    private float timer;
    private bool timerRunning = false;

    void Start()
    {
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
        // Aggiorna l'indice ogni volta che un oggetto viene distrutto
        objectsWithCollisionStateChanger.RemoveAll(obj => obj == null);

        TimerManager();
    }

    private void TimerManager()
    {
        // Gestione del timer
        if (timerRunning)
        {
            timer -= Time.deltaTime;

            // Aggiorna il testo del timer
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(timer).ToString();

                // Se il timer scende sotto il 50%, attiva l'oggetto TextMeshPro UI
                if (timer <= timerDuration / 2f)
                {
                    timerText.gameObject.SetActive(true);
                }
            }

            // Se il timer scade
            if (timer <= 0f)
            {
                timerRunning = false;
                EvaluateObjects();
            }
        }
    }

    private void OnStopDragging(Vector3 releaseVelocity)
    {
        // Fa partire il timer
        timer = timerDuration;
        timerRunning = true;

        //Debug.Log("Timer Partito");

        // Nascondi inizialmente il testo del timer
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
}
