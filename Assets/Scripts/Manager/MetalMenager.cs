using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalManager : MonoBehaviour
{
    // Riferimenti agli script "WreckingBallDrag" e "WreckingBallController"
    private WreckingBallDrag wreckingBallDrag;
    private WreckingBallController wreckingBallController;

    // Lista per contenere gli oggetti con il tag "Metallo" e le loro posizioni
    private List<MetalObject> metalObjects = new List<MetalObject>();

    // Struttura per memorizzare posizione e riferimento dell'oggetto
    private struct MetalObject
    {
        public GameObject obj;
        public Vector3 position;

        public MetalObject(GameObject _obj, Vector3 _position)
        {
            obj = _obj;
            position = _position;
        }
    }

    void Start()
    {
        // Trova gli script automaticamente
        wreckingBallDrag = FindFirstObjectByType<WreckingBallDrag>();
        wreckingBallController = FindFirstObjectByType<WreckingBallController>();

        // Verifica se sono stati trovati gli script
        if (wreckingBallDrag == null)
        {
            Debug.LogError("WreckingBallDrag script non trovato nella scena!");
        }

        if (wreckingBallController == null)
        {
            Debug.LogError("WreckingBallController script non trovato nella scena!");
        }

        // Trova tutti gli oggetti con il tag "Metallo" e salvatene la posizione
        GameObject[] metalObjectsArray = GameObject.FindGameObjectsWithTag("Metallo");

        foreach (GameObject metalObj in metalObjectsArray)
        {
            metalObjects.Add(new MetalObject(metalObj, metalObj.transform.position));
        }
    }

    void Update()
    {
        // Verifica se "isDragging" è vera
        if (wreckingBallDrag != null && wreckingBallDrag.isDragging)
        {
            // Controlla la distanza di ciascun oggetto "Metallo" dalla posizione del "WreckingBall"
            foreach (MetalObject metal in metalObjects)
            {
                float distance = Vector3.Distance(metal.obj.transform.position, wreckingBallController.transform.position);

                // Se la distanza è minore della variabile "distance" di WreckingBallController, cambia il layer
                if (distance < wreckingBallController.distance)
                {
                    // Cambia il layer dell'oggetto "Metallo" a "Muro"
                    metal.obj.layer = LayerMask.NameToLayer("Muro");
                }
                else
                {
                    metal.obj.layer = LayerMask.NameToLayer("Default");
                }
            }
        }
    }
}
