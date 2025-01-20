using UnityEngine;

public class ImpactHeadScript : MonoBehaviour
{
    [SerializeField] private Transform parentObject;
    private int impactHeadIndex = 2;
    private WreckingBallDrag wreckingBallDrag;

    public float lowImpactSpeed = 5f;
    public float highImpactSpeed = 10f;
    public GameObject objectToSpawn;
    public float spawnDistance = -2f;
    private Transform impactHead;
    private bool isScriptActive = false;

    [Header("Non Toccare")]
    public float impactHeadVelocity;

    [SerializeField] private string noCollisionLayerName = "NoCollision"; // Nome del layer NoCollision

    private void Start()
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent Object non assegnato! Assegna un parent nel campo 'Parent Object' nell'Inspector.");
            return;
        }

        if (parentObject.childCount > impactHeadIndex)
        {
            impactHead = parentObject.GetChild(impactHeadIndex);
        }
        else
        {
            Debug.LogError($"Il parent non ha abbastanza child. Assicurati che ci sia un child all'indice {impactHeadIndex}.");
        }

        wreckingBallDrag = GetComponent<WreckingBallDrag>();
        if (wreckingBallDrag == null)
        {
            Debug.LogError("WreckingBallDrag non trovato. Aggiungilo all'oggetto.");
        }
    }

    private void Update()
    {
        if (impactHead != null)
        {
            bool isImpactHeadActive = impactHead.gameObject.activeSelf;

            if (isImpactHeadActive && !isScriptActive)
            {
                isScriptActive = true;
                this.enabled = true;
            }
            else if (!isImpactHeadActive && isScriptActive)
            {
                isScriptActive = false;
                this.enabled = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Controlla se l'oggetto è nel layer NoCollision
        if (gameObject.layer == LayerMask.NameToLayer(noCollisionLayerName))
        {
            Debug.Log("Oggetto nel layer NoCollision, nessuna azione eseguita.");
            return;
        }

        if (isScriptActive && wreckingBallDrag.isSwinging)
        {
            float relativeSpeed = collision.relativeVelocity.magnitude;
            impactHeadVelocity = relativeSpeed;

            if (collision.gameObject.CompareTag("Metallo"))
            {
                ContactPoint contact = collision.contacts[0];
                Vector3 impactPoint = contact.point;
                Vector3 impactNormal = contact.normal;
                Vector3 spawnPoint = impactPoint + impactNormal * spawnDistance;

                Quaternion spawnRotation = Quaternion.LookRotation(collision.relativeVelocity.normalized, impactNormal);

                if (objectToSpawn != null)
                {
                    GameObject spawnedObject = Instantiate(objectToSpawn, spawnPoint, spawnRotation);

                    if (relativeSpeed <= lowImpactSpeed)
                    {
                        spawnedObject.transform.localScale = Vector3.one;
                    }
                    else if (relativeSpeed >= highImpactSpeed)
                    {
                        spawnedObject.transform.localScale = Vector3.one * 4f;
                    }
                    else
                    {
                        float scaleFactor = 1 + (relativeSpeed - lowImpactSpeed) / (highImpactSpeed - lowImpactSpeed) * 0.5f;
                        spawnedObject.transform.localScale = Vector3.one * scaleFactor;
                    }
                }
                else
                {
                    Debug.LogError("Nessun prefab assegnato a 'objectToSpawn'.");
                }
            }
        }
    }
}
