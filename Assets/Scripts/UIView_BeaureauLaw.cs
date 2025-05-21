// -----------------------------------------------------------------------------
// UIView_BeaureauLaw.cs
//
// MonoBehaviour for displaying law details and effects in the Bureau (Office) UI.
// Updates UI elements with the current law's title, description, and effects on factions.
// Listens for law effect changes and updates the display accordingly.
//
// Main Functions:
// - SetEffect(List<LawEffect> effects): Updates the UI with the current law's effects.
// - ShowLaw(): Shows the law details panel.
// - HideLaw(): Hides the law details panel.
//
// Fields:
// - _title, _description, _effect: UI text fields for law info.
// - _layout: Layout container for UI updates.
// - _openLawButton: Button to open the law panel.
// - _lawManager: Reference to the LawManager for effect updates.
// -----------------------------------------------------------------------------

using UnityEngine;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
public class UIView_BeaureauLaw : MonoBehaviour
{
    [Foldout("Components")] public TextMeshProUGUI _title;
    [Foldout("Components")] public TextMeshProUGUI _description;
    [Foldout("Components")] public TextMeshProUGUI _effect;
    [Foldout("Components")] public RectTransform _layout;
    [Foldout("References")] public GameObject _openLawButton;
    [Foldout("References")] public LawManager _lawManager;

    private void OnEnable()
    {
        var currentLaw = GameManager.Instance.CurrentLaw;
        _title.text = currentLaw.Name;
        _description.text = currentLaw.Description;
        SetEffect(_lawManager.CurrentLawEffects);

        _lawManager.OnLawEffectsChanged += OnLawEffectsChanged;
    }

    private void OnDisable()
    {
        _lawManager.OnLawEffectsChanged -= OnLawEffectsChanged;
    }

    private void OnLawEffectsChanged()
    {
        SetEffect(_lawManager.CurrentLawEffects);
    }

    public void SetEffect(List<LawEffect> effects)
    {
        _effect.gameObject.SetActive(effects is { Count: > 0 });

        if (effects == null || effects.Count == 0)
        {
            _effect.text = "";
            return;
        }

        var effectsTexts = new List<string>();
        foreach (var effect in effects)
        {
            var text = "";
            switch (effect.Value)
            {
                case 1:
                    text = "Favors";
                    break;
                case >= 2:
                    text = "Greatly Favors";
                    break;

                case -1:
                    text = "Opposes";
                    break;
                case <= -2:
                    text = "Greatly Opposes";
                    break;

                default:
                    continue;
            }

            effectsTexts.Add($"{text} {effect.Type.ToString()}");
        }

        _effect.text = string.Join("\n", effectsTexts);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_effect.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_layout);
    }

    public void ShowLaw()
    {
        _openLawButton.SetActive(false);
        gameObject.SetActive(true);
    }

    public void HideLaw()
    {
        if (_openLawButton != null)
        {
            _openLawButton.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}
