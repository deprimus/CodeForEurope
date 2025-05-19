using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class UIView_Vote : MonoBehaviour
{
    [Foldout("Components"), SerializeField] private RectTransform _greenBar;
    [Foldout("Components"), SerializeField] private TextMeshProUGUI _percentageText;
    [Foldout("Components"), SerializeField] private RectTransform _container;

    private Camera _camera;

    private float _greenBarWidth;

    private void Awake()
    {
        _greenBarWidth = _greenBar.sizeDelta.x;
        _container.localScale = Vector3.zero;
    }

    public async void SetPercentage(float percentage)
    {
        _percentageText.text = $"{(int)(percentage * 100)}%";
        _greenBar.sizeDelta = new Vector2(percentage * _greenBarWidth, _greenBar.sizeDelta.y);

        _container.DOScale(Vector3.one * 0.1f, 0.25f).SetEase(Ease.OutBack);

        await UniTask.Delay(2000);

        _container.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
    }

    private async void Start()
    {
        await UniTask.WaitUntil(() => CameraManager.Instance != null);
        await UniTask.WaitUntil(() => CameraManager.Instance.Camera != null);

        _camera = CameraManager.Instance.Camera;
        transform.LookAt(_camera.transform);
    }
}
