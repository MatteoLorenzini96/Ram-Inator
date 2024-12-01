using UnityEngine;

public class PendulumBehaviour : MonoBehaviour
{
    public Transform pivot; // Punto di ancoraggio dell'ariete
    public Rigidbody arieteRb; // Rigidbody dell'ariete
    public float ropeLength = 5.0f; // Lunghezza della corda
    public float damping = 0.98f; // Smorzamento del movimento per ridurre oscillazioni residue

    private Vector3 dragOffset; // Offset tra il mouse e la posizione dell'ariete
    private bool isDragging = false;

void Start()
    {
        // Configura la posizione iniziale dell'ariete in relazione al pivot
        Vector3 initialPosition = pivot.position + Vector3.down * ropeLength;
    arieteRb.transform.position = initialPosition;

        // Attiva la gravità sul Rigidbody
        arieteRb.useGravity = true;

        // Blocca alcune rotazioni del Rigidbody
        arieteRb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;

        // Crea il joint per limitare il movimento entro la lunghezza della corda
        var joint = arieteRb.gameObject.AddComponent<ConfigurableJoint>();
    joint.connectedBody = null; // Il pivot è un punto fisso
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = pivot.position;
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        // Imposta il limite di distanza massima
        var limit = joint.linearLimit;
    limit.limit = ropeLength;
        joint.linearLimit = limit;
    }

void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Controlla se il mouse è sull'ariete
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == arieteRb.gameObject)
            {
                isDragging = true;
                dragOffset = arieteRb.position - GetMouseWorldPosition();
            }
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            // Fine trascinamento
            isDragging = false;
        }

        if (isDragging)
        {
            DragAriete();
        }

        // Applica smorzamento al movimento
        arieteRb.linearVelocity *= damping;
    }

    private void DragAriete()
    {
        // Ottieni la posizione del mouse nello spazio 3D
        Vector3 mousePosition = GetMouseWorldPosition();

        // Calcola la nuova posizione dell'ariete sulla sfera attorno al pivot
        Vector3 direction = (mousePosition + dragOffset - pivot.position).normalized;
        Vector3 newPosition = pivot.position + direction * ropeLength;

        // Sposta l'ariete alla posizione calcolata
        arieteRb.MovePosition(newPosition);
    }

    private void SimulatePendulumMotion()
    {
        // Mantieni l'ariete alla distanza fissa dal pivot durante l'oscillazione
        Vector3 direction = (arieteRb.position - pivot.position).normalized;
        arieteRb.position = pivot.position + direction * ropeLength;
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Usa un piano per calcolare la posizione del mouse nello spazio 3D
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.back, pivot.position);
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return arieteRb.position; // Ritorna la posizione attuale se il calcolo fallisce
    }
}
