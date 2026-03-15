using UnityEngine;

public class LibraryCamera : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void MoveToLaptop()
    {
        _animator.Play("laptop");
    }

    public void OpenLaptopUI()
    {
        EuroChat.Instance.Show();
    }
}
