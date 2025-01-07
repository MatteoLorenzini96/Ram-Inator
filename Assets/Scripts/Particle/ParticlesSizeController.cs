using UnityEngine;

public class ParticlesSizeController : MonoBehaviour
{

    private new ParticleSystem particleSystem;
    private Vector3 previousPosition;
    private float objectSpeed;

    public float maxSize = 2f;


    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        previousPosition = transform.position;
    }

    void Update()
    {
        //Calcola la velocità dell'oggetto confrontando la posizione attuale con la posizione precedente
        float distanceMoved = Vector3.Distance(transform.position, previousPosition);
        objectSpeed = distanceMoved / Time.deltaTime;

        //Modifica la dimenzione delle particelle in base alla velocità
        var main = particleSystem.main;
        main.startSize = Mathf.Clamp(objectSpeed * 0.1f, 0.1f, maxSize);

        //Salva la posizione attuale per il prossimo calcolo
        previousPosition = transform.position;
    }
}
