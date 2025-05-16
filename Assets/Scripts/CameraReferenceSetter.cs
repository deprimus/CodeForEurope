using UnityEngine;

public class CameraReferenceSetter : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        CameraManager.Instance.SetCameraReference(_camera);
    }
}
