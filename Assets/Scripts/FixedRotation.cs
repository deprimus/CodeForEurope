using UnityEngine;

public class FixedRotation : MonoBehaviour
{
    Quaternion rotation;
    Vector3 position;

    void Awake()
    {
        rotation = transform.rotation;
        position = transform.position;
    }

    void OnEnable()
    {
        transform.rotation = rotation;
        transform.position = position;
    }
}
