using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
public class LibraryView_Laptop : MonoBehaviour
{
    [Foldout("References")] public Transform _target;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private bool _isBeingUsed;

    private void Awake()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    private void OnMouseDown()
    {
        if (_isBeingUsed)
            return;

        UseLaptop();
    }

    private async void UseLaptop()
    {
        _isBeingUsed = true;
        transform.DOMove(_target.position, 0.5f).SetEase(Ease.OutCubic);
        transform.DORotate(_target.rotation.eulerAngles, 0.5f).SetEase(Ease.OutCubic);

        await UniTask.Delay(1000);

        transform.DOMove(_originalPosition, 0.5f).SetEase(Ease.InCubic);
        transform.DORotate(_originalRotation.eulerAngles, 0.5f).SetEase(Ease.InCubic);

        _isBeingUsed = false;

        LibraryManager.Instance.UseLaptop();
    }
}
