using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBookPage : MonoBehaviour
{
    public struct RulePageData
    {
        public string title;
        public string description;
        public string longDescription;
        public List<string> effects;

        public bool effectsAreShown;
    }

    public TextMeshProUGUI RuleNameText;
    public TextMeshProUGUI RuleLongDescriptionText;
    public TextMeshProUGUI RuleEffectsText;

    public TextMeshProUGUI RuleEffectsLabelText;

    public Image EffectsBackgroundImage;

    public List<Button> debunkButtons;
    private List<bool> _displayDebunkButtons;

    private List<string> _initialEffectsText;
    private List<string> _debunkedEffectsText;
    private int _debunkedEffectsAllowed = 0;
    private bool _effectsAreShown;
    public void SetPageData(RulePageData pageData)
    {
        _displayDebunkButtons = new List<bool>();
        for(int i = 0; i < debunkButtons.Count; i++)
        {
            _displayDebunkButtons.Add(false);
        }
        RuleNameText.text = pageData.title;
        RuleLongDescriptionText.text = pageData.description + "\n" + pageData.longDescription;
        _initialEffectsText = pageData.effects;
        _effectsAreShown = pageData.effectsAreShown;
        if(_effectsAreShown)
        {
            _debunkedEffectsAllowed = 0;
            _debunkedEffectsText = _initialEffectsText;
            RuleEffectsText.text = string.Join("\n", _debunkedEffectsText);
            for(int i = 0; i < _displayDebunkButtons.Count; i++)
            {
                _displayDebunkButtons[i] = false;
                debunkButtons[i].gameObject.SetActive(_displayDebunkButtons[i]);
            }
        }
        else
        {
            _debunkedEffectsAllowed = Mathf.CeilToInt(_initialEffectsText.Count / 2f);
            _debunkedEffectsText = new List<string>();
            for(int i = 0; i < _initialEffectsText.Count; i++)
            {
                _displayDebunkButtons[i] = true;
                _debunkedEffectsText.Add("???");
                debunkButtons[i].gameObject.SetActive(_displayDebunkButtons[i]);
            }
            RuleEffectsText.text = string.Join("\n", _debunkedEffectsText);
        }

        ShowTextPage();
    }

    public void HideTextPage()
    {
        RuleNameText.gameObject.SetActive(false);
        RuleLongDescriptionText.gameObject.SetActive(false);
        RuleEffectsText.gameObject.SetActive(false);
        RuleEffectsLabelText.gameObject.SetActive(false);
        EffectsBackgroundImage.gameObject.SetActive(false);
        for(int i = 0; i < _displayDebunkButtons.Count; i++)
        {
            debunkButtons[i].gameObject.SetActive(false);
        }
    }

    public void ShowTextPage()
    {
        RuleNameText.gameObject.SetActive(true);
        RuleLongDescriptionText.gameObject.SetActive(true);
        RuleEffectsLabelText.gameObject.SetActive(true);
        RuleEffectsText.gameObject.SetActive(true);
        EffectsBackgroundImage.gameObject.SetActive(true);
        for(int i = 0; i < _displayDebunkButtons.Count; i++)
        {
            debunkButtons[i].gameObject.SetActive(_displayDebunkButtons[i]);
        }
    }

    public void DebunkEffects(int buttonIndex)
    {
        _effectsAreShown = true;
        //RuleEffectsText.text = _effectsText;
        debunkButtons[buttonIndex].gameObject.SetActive(false);
        _debunkedEffectsText[buttonIndex] = _initialEffectsText[buttonIndex];
        RuleEffectsText.text = string.Join("\n", _debunkedEffectsText);
        if(--_debunkedEffectsAllowed == 0)
        {
            for(int i = 0; i < _displayDebunkButtons.Count; i++)
            {
                debunkButtons[i].gameObject.SetActive(false);
                _displayDebunkButtons[i] = false;
            }
        }
    }
}
