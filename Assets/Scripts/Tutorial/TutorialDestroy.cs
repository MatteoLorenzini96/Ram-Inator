using UnityEngine;

public class TutorialDestroy : MonoBehaviour
{
    private WreckingBallDrag wreckingBallDrag;

    void Start()
    {
        wreckingBallDrag = FindFirstObjectByType<WreckingBallDrag>();
    }

    void Update()
    {
        if (wreckingBallDrag != null && wreckingBallDrag.isDragging)
        {
            Invoke("Explode", 2f);
        }
    }

    private void Explode()
    {
        Destroy(gameObject);
    }
}
