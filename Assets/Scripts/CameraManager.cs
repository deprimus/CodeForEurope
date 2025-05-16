using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public Camera Camera => _camera;

    private Camera _camera;

    public CameraManager()
    {
        Instance = this;
    }

    public void SetCameraReference(Camera camera)
    {
        _camera = camera;
    }
}
