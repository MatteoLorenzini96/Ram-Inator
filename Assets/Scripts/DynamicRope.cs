using UnityEngine;

public class DynamicRope : MonoBehaviour
{
    public GameObject segmentPrefab;  // Prefab del segmento della corda
    public int numberOfSegments = 10; // Numero di segmenti della corda (iniziale)
    public float ropeMass = 0.1f;     // Massa di ciascun segmento
    public float jointSpring = 50f;   // Forza del giunto
    public float jointDamper = 5f;    // Damping del giunto
    public float minSegmentLength = 0.2f; // Lunghezza minima del segmento
    public float maxSegmentLength = 2f;  // Lunghezza massima del segmento per ottimizzare la performance

    public Transform pointA;  // Primo punto di ancoraggio (es. un oggetto o un punto fisso)
    public Transform pointB;  // Secondo punto di ancoraggio (es. un altro oggetto o punto fisso)

    private GameObject[] segments;
    private ConfigurableJoint[] joints;

    void Start()
    {
        // Crea la corda tra i due punti di ancoraggio
        CreateRope();
    }

    void Update()
    {
        // Aggiorna la corda se i punti di ancoraggio si spostano
        UpdateRope();
    }

    void CreateRope()
    {
        // Calcola la distanza tra i due punti di ancoraggio
        float ropeLength = Vector3.Distance(pointA.position, pointB.position);

        // Calcola la lunghezza ideale di ogni segmento (ottimizzata)
        float segmentLength = Mathf.Clamp(ropeLength / numberOfSegments, minSegmentLength, maxSegmentLength);

        // Calcola il numero effettivo di segmenti in base alla distanza tra i punti
        numberOfSegments = Mathf.CeilToInt(ropeLength / segmentLength);

        // Inizializza gli array per i segmenti e i giunti
        segments = new GameObject[numberOfSegments];
        joints = new ConfigurableJoint[numberOfSegments - 1];

        // Crea il primo segmento, che è ancorato al primo punto
        segments[0] = Instantiate(segmentPrefab, pointA.position, Quaternion.identity);
        segments[0].GetComponent<Rigidbody>().mass = ropeMass;

        // Collega il primo segmento al punto di ancoraggio A con un FixedJoint
        FixedJoint fixedJointA = segments[0].AddComponent<FixedJoint>();
        fixedJointA.connectedBody = pointA.GetComponent<Rigidbody>();

        // Crea il resto dei segmenti
        for (int i = 1; i < numberOfSegments; i++)
        {
            // Crea il segmento
            segments[i] = Instantiate(segmentPrefab, pointA.position + new Vector3(0, segmentLength * i, 0), Quaternion.identity);
            segments[i].GetComponent<Rigidbody>().mass = ropeMass;

            // Collega il segmento precedente con un ConfigurableJoint
            joints[i - 1] = segments[i].AddComponent<ConfigurableJoint>();
            joints[i - 1].connectedBody = segments[i - 1].GetComponent<Rigidbody>();
            joints[i - 1].xMotion = ConfigurableJointMotion.Limited;
            joints[i - 1].yMotion = ConfigurableJointMotion.Limited;
            joints[i - 1].zMotion = ConfigurableJointMotion.Limited;

            // Impostazioni della molla
            SoftJointLimitSpring spring = new SoftJointLimitSpring
            {
                spring = jointSpring,
                damper = jointDamper
            };
            joints[i - 1].linearLimitSpring = spring;

            // Limiti di movimento per simulare una corda
            joints[i - 1].linearLimit = new SoftJointLimit
            {
                limit = segmentLength
            };
        }

        // Collega l'ultimo segmento al punto di ancoraggio B con un FixedJoint
        FixedJoint fixedJointB = segments[numberOfSegments - 1].AddComponent<FixedJoint>();
        fixedJointB.connectedBody = pointB.GetComponent<Rigidbody>();
    }

    void UpdateRope()
    {
        // Calcola la distanza tra i punti di ancoraggio
        float ropeLength = Vector3.Distance(pointA.position, pointB.position);

        // Calcola la lunghezza ideale di ogni segmento (ottimizzata)
        float segmentLength = Mathf.Clamp(ropeLength / numberOfSegments, minSegmentLength, maxSegmentLength);

        // Calcola il numero effettivo di segmenti in base alla distanza tra i punti
        int updatedSegments = Mathf.CeilToInt(ropeLength / segmentLength);

        // Se il numero di segmenti è cambiato, aggiorniamo la corda
        if (updatedSegments != numberOfSegments)
        {
            // Distruggiamo i segmenti esistenti
            foreach (var segment in segments)
            {
                Destroy(segment);
            }

            // Ricreiamo la corda con il nuovo numero di segmenti
            numberOfSegments = updatedSegments;
            CreateRope();
        }

        // Ricalcola la posizione dei segmenti
        for (int i = 0; i < numberOfSegments; i++)
        {
            if (i == 0)
            {
                segments[i].transform.position = pointA.position;
            }
            else if (i == numberOfSegments - 1)
            {
                segments[i].transform.position = pointB.position;
            }
            else
            {
                segments[i].transform.position = Vector3.Lerp(pointA.position, pointB.position, (float)i / (numberOfSegments - 1));
            }
        }
    }

    // Visualizza la corda in editor usando i Gizmos
    void OnDrawGizmos()
    {
        if (segments == null || segments.Length == 0) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < segments.Length - 1; i++)
        {
            Gizmos.DrawLine(segments[i].transform.position, segments[i + 1].transform.position);
        }
    }
}
