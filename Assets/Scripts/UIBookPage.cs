using TMPro;
using UnityEngine;

public class UIBookPage : MonoBehaviour
{
    public struct RulePageData
    {
        public string title;
        public string description;
        public string longDescription;
        public string effects;
    }

    public TextMeshProUGUI RuleNameText;
    public TextMeshProUGUI RuleDescriptionText;
    public TextMeshProUGUI RuleLongDescriptionText;
    public TextMeshProUGUI RuleEffectsText;
    
    public void SetPageData(RulePageData pageData)
    {
        RuleNameText.text = pageData.title;
        RuleDescriptionText.text = pageData.description;
        RuleLongDescriptionText.text = pageData.longDescription;
        RuleEffectsText.text = pageData.effects;
        ShowTextPage();
    }

    public void HideTextPage()
    {
        RuleNameText.gameObject.SetActive(false);
        RuleDescriptionText.gameObject.SetActive(false);
        RuleLongDescriptionText.gameObject.SetActive(false);
        RuleEffectsText.gameObject.SetActive(false);
    }

    public void ShowTextPage()
    {
        RuleNameText.gameObject.SetActive(true);
        RuleDescriptionText.gameObject.SetActive(true);
        RuleLongDescriptionText.gameObject.SetActive(true);
        RuleEffectsText.gameObject.SetActive(true);
    }
}
