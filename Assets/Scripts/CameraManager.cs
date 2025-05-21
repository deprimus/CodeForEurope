// -----------------------------------------------------------------------------
// CameraManager.cs
//
// Manages the main camera reference and switching between camera views as needed.
// Implements a singleton pattern for global access.
//
// Main Functions:
// - SetCameraReference(Camera camera): Sets the current camera reference.
//
// Fields:
// - _camera: The current active camera.
// -----------------------------------------------------------------------------

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
