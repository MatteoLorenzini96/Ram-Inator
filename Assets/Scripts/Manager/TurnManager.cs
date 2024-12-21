using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    private TimeManager timeManager;
    public int maxAttempts = 5; // Numero massimo di tentativi impostabile dall'inspector
    public TextMeshProUGUI attemptsText;
    public GameObject evaluetePanel;


    private Transform cameraParent; // Variabile per il parent della telecamera
    private List<GameObject> objectsWithCollisionStateChanger;
    private int initialObjectCount;
    private int remainingAttempts;
    private Vector3 lastHitPosition; // Posizione dell'ultimo oggetto colpito

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

        // Cerca WreckingBallDrag e si iscrive al suo evento OnRelease
        WreckingBallDrag wreckingBallDrag = FindFirstObjectByType<WreckingBallDrag>();
        if (wreckingBallDrag != null)
        {
            wreckingBallDrag.OnRelease += OnStopDragging;
        }
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

    private void OnStopDragging(Vector3 releaseVelocity)
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

        evaluetePanel.SetActive(true);
        
        int remainingObjects = objectsWithCollisionStateChanger.Count;
        float percentage = (float)remainingObjects / initialObjectCount * 100f;

        int stars = 0;

        if (percentage > 50f)
        {
            Debug.Log("fai schifo");
            stars = 1;
        }
        else if (percentage <= 50f && percentage > 25f)
        {
            Debug.Log("meh");
            stars = 2;
        }
        else if (percentage <= 25f && percentage > 0f)
        {
            Debug.Log("Skill Issue");
            stars = 3;
        }
        else if (percentage == 0f)
        {
            Debug.Log("GG EZ");
            stars = 4;
        }

        // Salva il punteggio nel GameManager
        GameManager.Instance.SetLevelStars(SceneManager.GetActiveScene().buildIndex, stars);

        // Carica l'hub dopo il completamento
        SceneManager.LoadScene("CentralHub");
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
