using UnityEngine;

public class ChildCollision : MonoBehaviour
{
    private ParentCollisionManager padre;

    public float timeToDestroy = 5f;

    private void Start()
    {
        // Trova il componente ParentTriggerManager sul padre
        padre = GetComponentInParent<ParentCollisionManager>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (padre != null)
        {
            // Notifica il padre del trigger
            padre.GestisciTrigger(other, this);

            Invoke("DestroyObject", timeToDestroy);
        }
    }

    void DestroyObject()
    {
        // Distruggere l'oggetto
        Destroy(gameObject);
    }
}
