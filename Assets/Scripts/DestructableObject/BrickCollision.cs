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

    private void HandleCollision(GameObject other)
    {
        // Verifica se l'oggetto con cui si è in collisione è nel Layer "Mattoni"
        if (other.layer == LayerMask.NameToLayer("Mattoni"))
        {
            // Calcola la velocità relativa dell'oggetto che entra in collisione
            Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                float relativeSpeed = otherRigidbody.linearVelocity.magnitude;

                // Gestisce il cambiamento di stato in base alla velocità relativa
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
                    collisionStateChanger.Explode(relativeSpeed);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }
}
