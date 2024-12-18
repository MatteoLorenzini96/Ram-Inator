using UnityEngine;

public class SlideAncor : MonoBehaviour
{
    // Riferimenti agli oggetti che definiscono i limiti
    public Transform puntoAObject; // Oggetto per il limite sinistro
    public Transform puntoBObject; // Oggetto per il limite destro

    private float puntoA;          // Valore calcolato della posizione X di puntoAObject
    private float puntoB;          // Valore calcolato della posizione X di puntoBObject
    private Transform selectedObject; // Oggetto selezionato
    private float offsetX;         // Offset tra il tocco/mouse e l'oggetto
    private Camera mainCamera;     // Riferimento alla camera principale

    public bool anchorMoving = false;
    void Start()
    {
        // Cache della camera principale
        mainCamera = Camera.main;

        // Calcola i limiti iniziali basati sulle posizioni degli oggetti
        if (puntoAObject != null && puntoBObject != null)
        {
            puntoA = puntoAObject.position.x;
            puntoB = puntoBObject.position.x;
        }
        else
        {
            Debug.LogError("Assicurati di assegnare i riferimenti a PuntoAObject e PuntoBObject.");
        }
    }

    void Update()
    {
        // Controlla input mouse o touch
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            // Lancia un raycast per selezionare l'oggetto
            Vector3 inputPosition = GetInputPosition();
            Ray ray = mainCamera.ScreenPointToRay(inputPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Controlla se l'oggetto colpito è trascinabile
                if (hit.transform.GetComponent<SlideAncor>() != null)
                {
                    selectedObject = hit.transform;

                    // Calcola l'offset tra la posizione del tocco/mouse e l'oggetto
                    Vector3 worldInputPosition = mainCamera.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, mainCamera.WorldToScreenPoint(selectedObject.position).z));
                    offsetX = selectedObject.position.x - worldInputPosition.x;

                    anchorMoving = true;                     // Imposta anchorMoving a true
                }
            }
        }

        // Controlla trascinamento
        if (selectedObject != null && (Input.GetMouseButton(0) || Input.touchCount > 0))
        {
            Vector3 inputPosition = GetInputPosition();
            Vector3 worldInputPosition = mainCamera.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, mainCamera.WorldToScreenPoint(selectedObject.position).z));

            // Calcola la nuova posizione lungo l'asse X
            float newX = worldInputPosition.x + offsetX;

            // Limita la posizione tra puntoA e puntoB
            newX = Mathf.Clamp(newX, puntoA, puntoB);

            // Aggiorna la posizione dell'oggetto
            selectedObject.position = new Vector3(newX, selectedObject.position.y, selectedObject.position.z);
        }

        // Rilascia l'oggetto
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            selectedObject = null;

            anchorMoving = false;                     // Imposta anchorMoving a false
        }
    }

    // Metodo per ottenere la posizione dell'input (mouse o touch)
    private Vector3 GetInputPosition()
    {
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        }
        else
        {
            return Input.mousePosition;
        }
    }
}
