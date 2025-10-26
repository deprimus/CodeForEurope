using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
public class UIView_Mood : MonoBehaviour
{
    [Foldout("Attributes")] public Sprite[] _moodSprites;
    [Foldout("Components")] public Image _moodImage;
    [Foldout("Components")] public RectTransform _moodTransform;

    private Camera _camera;

    private void Awake()
    {
        _moodTransform.localScale = Vector3.zero;
    }

    private async void Start()
    {
        await UniTask.WaitUntil(() => CameraManager.Instance != null);
        await UniTask.WaitUntil(() => CameraManager.Instance.Camera != null);

        _camera = CameraManager.Instance.Camera;
        transform.LookAt(_camera.transform);
    }

    public async void ShowMood(Mood mood)
    {
        _moodImage.sprite = _moodSprites[(int)mood];

        _moodTransform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);

        await UniTask.Delay(2000);

        _moodTransform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
    }
}

public enum Mood
{
    Happy,
    Angry,
    Neutral
}
