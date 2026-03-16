using UnityEngine;

public class LibraryCamera : MonoBehaviour
{
    private Animator _animator;
    private Vector3 _savedPosition;
    private Quaternion _savedRotation;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void MoveToLaptop()
    {
        _savedPosition = transform.position;
        _savedRotation = transform.rotation;
        _animator.Play("laptop");
    }

    public void RestoreCamera()
    {
        _animator.Play("idle");
        transform.position = _savedPosition;
        transform.rotation = _savedRotation;
    }

    public void OpenLaptopUI()
    {
        EuroChat.Instance.Show();
    }
}
