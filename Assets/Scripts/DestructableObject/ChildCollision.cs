using UnityEngine;

public class ChildCollision : MonoBehaviour
{
    private ParentCollisionManager padre;

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
        }
    }
}
