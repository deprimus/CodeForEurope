using UnityEngine;
using NaughtyAttributes;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading.Tasks;
public class UIView_VotePicker : MonoBehaviour
{
    [Foldout("Components")] public CanvasGroup _canvasGroup;
    [Foldout("Components")] public TextMeshProUGUI _title;
    [Foldout("Components")] public CanvasGroup _votesCanvasGroup;

    private Action<bool> _onVotePicked;

    private void Awake()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _title.alpha = 0;
        _votesCanvasGroup.alpha = 0;
    }

    public async Task Show(Action<bool> onVotePicked)
    {
        _onVotePicked = onVotePicked;

        _canvasGroup.DOFade(1, 0.33f).SetEase(Ease.OutCubic);

        await UniTask.Delay(250);

        _title.DOFade(1, 0.33f).SetEase(Ease.OutCubic);

        await UniTask.Delay(100);

        _votesCanvasGroup.DOFade(1, 0.33f).SetEase(Ease.OutCubic);

        await UniTask.Delay(330);

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    public void PickVote(int vote)
    {
        PickVoteAsync(vote == 1);
    }

    private async Task PickVoteAsync(bool vote)
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _canvasGroup.DOFade(0, 0.33f).SetEase(Ease.InCubic);
        _title.DOFade(0, 0.33f).SetEase(Ease.InCubic);
        _votesCanvasGroup.DOFade(0, 0.33f).SetEase(Ease.InCubic);

        SoundManager.instance.Play(SoundManager.instance.select);

        await UniTask.Delay(250);

        _onVotePicked?.Invoke(vote);
    }
}
