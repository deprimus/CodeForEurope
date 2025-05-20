
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_ChoiceCard : MonoBehaviour
{
    [Foldout("Components")] public TextMeshProUGUI _descriptionText;
    [Foldout("Components")] public TextMeshProUGUI _effectsText;
    [Foldout("Components")] public RectTransform _cardFront;
    [Foldout("Components")] public RectTransform _cardBack;
    [Foldout("Components")] public Button _revertButton;

    public BureauChoice Choice { get; private set; }

    public void Initialize(BureauChoice choice)
    {
        Choice = choice;
        _descriptionText.text = choice.Description;
        _cardBack.gameObject.SetActive(false);
        _revertButton.gameObject.SetActive(false);
    }

    public async void Reveal()
    {
        // Flip animation
        transform.DORotate(new Vector3(0, 90, 0), 0.3f).SetEase(Ease.InOutQuad);

        await UniTask.Delay(300);

        _cardFront.gameObject.SetActive(false);
        _cardBack.gameObject.SetActive(true);

        // Show effects
        string effectsText = "";
        foreach (var effect in Choice.Effects)
        {
            effectsText += $"{(effect.Value >= 0 ? "+" : "")}{effect.Value} {effect.Type}\n";
        }
        _effectsText.text = effectsText;

        transform.DORotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.InOutQuad);
        Choice.IsRevealed = true;
        _revertButton.gameObject.SetActive(true);
    }

    public void OnRevertClicked()
    {
        LibraryManager.Instance.RevertChoice(this);
    }
}