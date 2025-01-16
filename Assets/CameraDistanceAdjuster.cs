using UnityEngine;

public class CameraParentDistanceAdjuster : MonoBehaviour
{
    public Camera cam;          // La Camera che deve adattarsi
    public Transform target;    // Il Plane da inglobare (oggetto 3D)
    public Transform cameraParent;  // Il parent della Camera che sposterai
    public float padding = 1f;  // Margine aggiuntivo per evitare che l'oggetto sia troppo vicino ai bordi

    void Start()
    {
        AdjustCameraParentDistance();
    }

    void AdjustCameraParentDistance()
    {
        // Ottieni il Renderer del target per ottenere le sue dimensioni
        Renderer targetRenderer = target.GetComponent<Renderer>();
        if (targetRenderer == null)
        {
            Debug.LogError("L'oggetto target deve avere un componente Renderer.");
            return;
        }

        // Calcola le dimensioni del Plane (target)
        float targetHeight = targetRenderer.bounds.size.y;  // Altezza del Plane
        float targetWidth = targetRenderer.bounds.size.x;   // Larghezza del Plane

        // Usa il FOV attuale della Camera per calcolare la distanza necessaria
        float fov = cam.fieldOfView;
        float aspectRatio = (float)Screen.width / (float)Screen.height;

        // Calcola la distanza per l'asse X (larghezza) e Y (altezza)
        float distanceForHeight = targetHeight / (2 * Mathf.Tan(fov * Mathf.Deg2Rad / 2));
        float distanceForWidth = (targetWidth / 2) / (Mathf.Tan(fov * Mathf.Deg2Rad / 2) * aspectRatio);

        // Prendi la distanza maggiore tra larghezza e altezza per assicurarti che il Plane sia completamente visibile
        float desiredDistance = Mathf.Max(distanceForHeight, distanceForWidth) + padding;

        // Regola la posizione del parent della Camera lungo l'asse Z (profondità)
        Vector3 newPosition = cameraParent.position;
        newPosition.z = -desiredDistance; // Posiziona il parent in modo che il Plane sia visibile
        cameraParent.position = newPosition;

        // Assicurati che la Camera guardi sempre verso il target (il Plane)
        cam.transform.LookAt(target);
    }
}
