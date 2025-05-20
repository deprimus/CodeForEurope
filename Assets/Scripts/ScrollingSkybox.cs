using UnityEngine;

public class ScrollingSkybox : MonoBehaviour
{
    [SerializeField]
    Material skybox;

    float angle;

    void Awake()
    {
        angle = 0f;
    }

    void Update()
    {
        angle = (angle + Time.deltaTime * 1f) % 360f;
        skybox.SetFloat("_Rotation", angle);
    }
}
