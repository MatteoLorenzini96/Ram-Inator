using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    // Tempo in secondi prima che l'oggetto venga distrutto
    [SerializeField] // Rende modificabile il campo nell'Inspector
    private float timeToDestroy = 5f;

    private void Start()
    {
        // Avvia il conto alla rovescia per distruggere l'oggetto
        Destroy(gameObject, timeToDestroy);
    }
}
