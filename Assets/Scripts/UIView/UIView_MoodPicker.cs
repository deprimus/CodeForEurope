using UnityEngine;
using System;
using TMPro;
using NaughtyAttributes;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
public class UIView_MoodPicker : MonoBehaviour
{
    [Foldout("Components")] public CanvasGroup _canvasGroup;
    [Foldout("Components")] public TextMeshProUGUI _title;
    [Foldout("Components")] public CanvasGroup _moodsCanvasGroup;

    private Action<Mood> _onMoodPicked;

    private void Awake()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _title.alpha = 0;
        _moodsCanvasGroup.alpha = 0;
    }

    public async Task Show(Action<Mood> onMoodPicked)
    {
        _onMoodPicked = onMoodPicked;

        _canvasGroup.DOFade(1, 0.33f).SetEase(Ease.OutCubic);

        await UniTask.Delay(250);

        _title.DOFade(1, 0.33f).SetEase(Ease.OutCubic);

        await UniTask.Delay(100);

        _moodsCanvasGroup.DOFade(1, 0.33f).SetEase(Ease.OutCubic);

        await UniTask.Delay(330);

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    public void PickMood(int mood)
    {
        PickMoodAsync((Mood)mood);
    }

    private async Task PickMoodAsync(Mood mood)
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _canvasGroup.DOFade(0, 0.33f).SetEase(Ease.InCubic);
        _title.DOFade(0, 0.33f).SetEase(Ease.InCubic);
        _moodsCanvasGroup.DOFade(0, 0.33f).SetEase(Ease.InCubic);

        SoundManager.instance.Play(SoundManager.instance.select);

        await UniTask.Delay(250);

        _onMoodPicked?.Invoke((Mood)mood);
    }
}
