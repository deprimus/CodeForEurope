using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class UIView_Law : MonoBehaviour
{
    [Foldout("Components")] public CanvasGroup _canvasGroup;
    [Foldout("Components")] public RectTransform _transform;
    [Foldout("Components")] public TextMeshProUGUI _title;
    [Foldout("Components")] public TextMeshProUGUI _description;
    [Foldout("Components")] public TextMeshProUGUI _effect;

    private void Awake()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        _transform.localScale = Vector3.one * 0.75f;
    }

    public async void ShowLaw(Law law)
    {
        _title.text = law.Name;
        _description.text = law.Description;
        _effect.text = law.Effect;

        await UniTask.Delay(500);

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        _canvasGroup.DOFade(1, 0.25f).SetEase(Ease.OutCubic);
        _transform.DOScale(1, 0.25f).SetEase(Ease.OutCubic).ChangeStartValue(Vector3.one * 0.75f);
    }

    public async void HideLaw()
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _canvasGroup.DOFade(0, 0.25f).SetEase(Ease.InCubic);
        _transform.DOScale(0.75f, 0.25f).SetEase(Ease.InCubic);

        await UniTask.Delay(500);

        GameManager.Instance.OnLawCardHidden();
    }
}
