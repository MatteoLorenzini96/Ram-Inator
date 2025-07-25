using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegLen;
    private int segmentLength;
    public float lineWidth = 0.15f;

    public Transform startPoint; // Assegna il punto Start dalla scena
    public Transform endPoint;   // Assegna il punto End dalla scena

    private WreckingBallController wreckingBallController;

    // Use this for initialization
    void Start()
    {
        // Recupera la variabile "distance" dallo script WreckingBallController
        wreckingBallController = FindFirstObjectByType<WreckingBallController>();
        if (wreckingBallController == null)
        {
            Debug.LogError("WreckingBallController non trovato nella scena.");
            return;
        }

        UpdateRope(wreckingBallController);
    }

    private void UpdateRope(WreckingBallController wreckingBallController)
    {
        float distance = wreckingBallController.distance;
        segmentLength = Mathf.CeilToInt(distance / 0.25f); // Numero segmenti basato sulla lunghezza della corda
        ropeSegLen = distance / segmentLength; // Lunghezza di ogni segmento

        this.lineRenderer = this.GetComponent<LineRenderer>();

        // Inizializza i segmenti della corda tra Start e End
        Vector2 start = startPoint.position;
        Vector2 end = endPoint.position;
        Vector2 direction = (end - start).normalized;

        for (int i = 0; i < segmentLength; i++)
        {
            Vector2 segmentPosition = start + direction * (ropeSegLen * i);
            this.ropeSegments.Add(new RopeSegment(segmentPosition));
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.DrawRope();
        UpdateRope(wreckingBallController);
    }

    private void FixedUpdate()
    {
        this.Simulate();
    }

    private void Simulate()
    {
        // SIMULATION
        Vector2 forceGravity = new Vector2(0f, -1f);

        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            this.ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        // Constraint to Start Point
        RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.posNow = startPoint.position;
        this.ropeSegments[0] = firstSegment;

        // Constraint to End Point
        RopeSegment lastSegment = this.ropeSegments[this.segmentLength - 1];
        lastSegment.posNow = endPoint.position;
        this.ropeSegments[this.segmentLength - 1] = lastSegment;

        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector2 changeDir = Vector2.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                this.ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                this.ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this.segmentLength];
        for (int i = 0; i < this.segmentLength; i++)
        {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}