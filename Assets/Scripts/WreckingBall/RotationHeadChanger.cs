using UnityEngine;

public class RotationHeadChanger : MonoBehaviour
{
    public Vector3 rotation;
    public float speed;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation * speed * Time.deltaTime);
    }
}
