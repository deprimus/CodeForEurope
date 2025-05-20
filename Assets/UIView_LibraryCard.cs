using NaughtyAttributes;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Linq;

public class UIView_LibraryCard : MonoBehaviour
{
    [Foldout("Components"), SerializeField] private TextMeshProUGUI _title;
    [Foldout("Components"), SerializeField] private TextMeshProUGUI _effects;
    [Foldout("Components"), SerializeField] private GameObject _revertButton;
    [Foldout("Components"), SerializeField] private GameObject _backImage;

    private bool _revealed = false;
    private bool _debunked = false;

    private string _name;
    private List<InteractionEffect> _interactionEffects;
    private bool _option;

    private float _originalScale;

    public async void Reveal()
    {
        if (_revealed)
            return;

        _revealed = true;

        _originalScale = transform.localScale.x;

        transform.DOScaleX(-_originalScale, 0.2f).SetEase(Ease.Linear);

        await UniTask.Delay(200);

        _backImage.SetActive(false);

        transform.DOScaleX(_originalScale, 0.2f).SetEase(Ease.Linear);
    }

    public void Debunk()
    {
        if (_debunked)
            return;

        _debunked = true;

        SetData(_name, _interactionEffects, _option);

        _revertButton.SetActive(true);
    }

    public void Revert()
    {
        var effects = _interactionEffects;

        for (int i = 0; i < 2; i++)
        {
            foreach (var effect in effects)
            {
                var value = _option ? -effect.Value : effect.Value;
                RoundTableManager.Instance.Influence(effect.Type, value);
            }
        }

        LibraryManager.Instance.OnRevertApplied();
    }

    public void HideRevert()
    {
        _revertButton.SetActive(false);
    }

    public void SetData(string name, List<InteractionEffect> effects, bool option)
    {
        _title.text = name;
        _effects.text = GetEffectsText(effects);

        _name = name;
        _interactionEffects = effects;
        _option = option;
    }

    private string GetEffectsText(List<InteractionEffect> effects)
    {
        var str = "";
        foreach (var effect in effects)
        {
            var value = _option ? effect.Value : -effect.Value;
            var valueStr = _debunked ? ((value > 0 ? "+" : "") + value) : "?";
            var typeStr = _debunked ? string.Concat(effect.Type.ToString().Select(x => char.IsUpper(x) ? (" " + x) : x.ToString())).TrimStart() : "?";
            str += typeStr + ": " + valueStr + " Influence\n";
        }

        return str;
    }
}
