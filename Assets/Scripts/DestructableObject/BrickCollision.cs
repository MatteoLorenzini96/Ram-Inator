using UnityEngine;

public class BrickCollision : MonoBehaviour
{
    private CollisionStateChanger collisionStateChanger;  // Riferimento allo script CollisionStateChanger

    private void Awake()
    {
        // Ottieni il riferimento allo script CollisionStateChanger sull'oggetto
        collisionStateChanger = GetComponent<CollisionStateChanger>();

        if (collisionStateChanger == null)
        {
            Debug.LogError("CollisionStateChanger non trovato sullo stesso oggetto!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se l'oggetto con cui si � in collisione � nel Layer "Mattoni"
        if (other.gameObject.layer == LayerMask.NameToLayer("Mattoni"))
        {
            //Debug.Log("Collisione avvenuta");

            // Calcola la velocit� relativa dell'oggetto che entra nel trigger
            Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                // Usa la velocit� dell'oggetto con cui si � in collisione
                float relativeSpeed = otherRigidbody.linearVelocity.magnitude;

                // Gestisce il cambiamento di stato come se l'oggetto fosse una "Palla"
                if (relativeSpeed < collisionStateChanger.lowSpeedThreshold)
                {
                    collisionStateChanger.ChangeState(CollisionStateChanger.ObjectState.Idle);
                }
                else if (relativeSpeed >= collisionStateChanger.lowSpeedThreshold && relativeSpeed < collisionStateChanger.highSpeedThreshold)
                {
                    collisionStateChanger.ChangeState(CollisionStateChanger.ObjectState.LowImpact);
                    collisionStateChanger.viteoggetto -= 1;
                    AudioManager.Instance.PlaySFX(collisionStateChanger.soundEffectDanneggiato);
                }
                else if (relativeSpeed >= collisionStateChanger.highSpeedThreshold)
                {
                    collisionStateChanger.ChangeState(CollisionStateChanger.ObjectState.HighImpact);
                    collisionStateChanger.viteoggetto -= 2;
                }

                // Se le vite sono esaurite, attiva l'esplosione
                if (collisionStateChanger.viteoggetto <= 0)
                {
                    if (collisionStateChanger.replacementPrefab != null)
                    {
                        collisionStateChanger.Explode();
                    }
                }
            }
        }
    }
}
