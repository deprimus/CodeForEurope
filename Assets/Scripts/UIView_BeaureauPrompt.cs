using System;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class UIView_BeaureauPrompt : MonoBehaviour
{
    [Foldout("Components")] public CanvasGroup _canvasGroup;
    [Foldout("Components")] public TextMeshProUGUI _title;
    [Foldout("Components")] public CanvasGroup _buttonsCanvasGroup;

    private Action<bool> _onOptionPicked;

    private void Awake()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _title.alpha = 0;
        _buttonsCanvasGroup.alpha = 0;
    }

    public async Task Show(Action<bool> onOptionPicked)
    {
        _onOptionPicked = onOptionPicked;

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        _canvasGroup.DOFade(1, 0.33f).SetEase(Ease.OutCubic);

        await UniTask.Delay(250);

        _title.DOFade(1, 0.33f).SetEase(Ease.OutCubic);

        await UniTask.Delay(100);

        _buttonsCanvasGroup.DOFade(1, 0.33f).SetEase(Ease.OutCubic);
    }

    public void PickOption(int option)
    {
        PickOptionAsync((option == 1));
    }

    private async Task PickOptionAsync(bool option)
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _canvasGroup.DOFade(0, 0.33f).SetEase(Ease.InCubic);
        _title.DOFade(0, 0.33f).SetEase(Ease.InCubic);
        _buttonsCanvasGroup.DOFade(0, 0.33f).SetEase(Ease.InCubic);

        await UniTask.Delay(250);

        _onOptionPicked?.Invoke(option);
    }
}
