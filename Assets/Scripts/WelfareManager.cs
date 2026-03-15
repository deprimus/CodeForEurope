using System.Collections.Generic;
using UnityEngine;

public class WelfareManager
{
    public const float GDP_INIT = 10000f, GDP_MIN = 8000f, GDP_MAX = 13000f;
    public const float GINI_INIT = 0.40f, GINI_MIN = 0.00f, GINI_MAX = 1.00f;
    public const float HC_INIT = 5.0f, HC_MIN = 0.0f, HC_MAX = 10.0f;
    public const float LE_INIT = 72f, LE_MIN = 65f, LE_MAX = 85f;

    public float GDP { get; private set; }
    public float Gini { get; private set; }
    public float HumanCapital { get; private set; }
    public float LifeExpectancy { get; private set; }

    public WelfareManager() => Reset();

    public void Reset()
    {
        GDP = GDP_INIT;
        Gini = GINI_INIT;
        HumanCapital = HC_INIT;
        LifeExpectancy = LE_INIT;

        UpdateUI();
    }

    public void ApplyEffects(List<WelfareEffect> effects, float multiplier = 1f)
    {
        if (effects == null) return;
        foreach (var e in effects)
        {
            float val = e.Value * multiplier;
            switch (e.Indicator)
            {
                case WelfareIndicator.GDP:
                    GDP = Mathf.Clamp(GDP + val, GDP_MIN, GDP_MAX);
                    break;
                case WelfareIndicator.Gini:
                    Gini = Mathf.Clamp(Gini + val, GINI_MIN, GINI_MAX);
                    break;
                case WelfareIndicator.HumanCapital:
                    HumanCapital = Mathf.Clamp(HumanCapital + val, HC_MIN, HC_MAX);
                    break;
                case WelfareIndicator.LifeExpectancy:
                    LifeExpectancy = Mathf.Clamp(LifeExpectancy + val, LE_MIN, LE_MAX);
                    break;
            }
        }

        UpdateUI();
    }
    
    public float NormalizedGDP => (GDP - GDP_MIN) / (GDP_MAX - GDP_MIN);
    public float NormalizedGini => 1f - Gini;
    public float NormalizedHumanCapital => (HumanCapital - HC_MIN) / (HC_MAX - HC_MIN);
    public float NormalizedLifeExpectancy => (LifeExpectancy - LE_MIN) / (LE_MAX - LE_MIN);

    public float CompositeWelfareScore =>
        (NormalizedGDP + NormalizedGini + NormalizedHumanCapital + NormalizedLifeExpectancy) / 4f;

    public enum CompositeTier { Flourishing, Stagnant, Crumbling }

    public struct WelfareEnding
    {
        public CompositeTier Tier;
        public string TierTitle;
        public string TierNarration;
        public WelfareIndicator DominantIndicator;
        public string DominantTitle;
        public string DominantNarration;
    }

    public WelfareEnding GetEnding()
    {
        var score = CompositeWelfareScore;
        var ending = new WelfareEnding();

        // Composite tier
        if (score >= 0.52f)
        {
            ending.Tier = CompositeTier.Flourishing;
            ending.TierTitle = "Flourishing Europe";
            ending.TierNarration = "Against all odds, Europe flourishes. The economy grows, inequality shrinks, citizens are educated, and the air is clean. No single faction won \u2014 but everyone gained. History books will call this the European Renaissance. And in the footnotes, they'll mention the parliament that made it possible.";
        }
        else if (score >= 0.40f)
        {
            ending.Tier = CompositeTier.Stagnant;
            ending.TierTitle = "Stagnant Europe";
            ending.TierNarration = "Nothing changed. Twenty laws debated, voted, argued \u2014 and Europe ends exactly where it started. Not worse, not better. Just... the same. Citizens shrug and carry on. Your parliament proved that doing something and doing nothing can look remarkably alike.";
        }
        else
        {
            ending.Tier = CompositeTier.Crumbling;
            ending.TierTitle = "Crumbling Europe";
            ending.TierNarration = "Every indicator falls. The economy contracts, inequality widens, education declines, and health deteriorates. Europe doesn't collapse with a bang \u2014 it erodes, slowly, law by law, vote by vote. Your parliament didn't fail spectacularly. It failed quietly. And that's worse.";
        }

        // Dominant metric: best indicator if flourishing, worst if stagnant/crumbling
        float[] values = { NormalizedGDP, NormalizedGini, NormalizedHumanCapital, NormalizedLifeExpectancy };
        bool pickBest = ending.Tier == CompositeTier.Flourishing;

        int dominant = 0;
        for (int i = 1; i < 4; i++)
        {
            if (pickBest ? values[i] > values[dominant] : values[i] < values[dominant])
                dominant = i;
        }

        ending.DominantIndicator = (WelfareIndicator)dominant;

        if (pickBest)
        {
            switch (ending.DominantIndicator)
            {
                case WelfareIndicator.GDP:
                    ending.DominantTitle = "GDP Titan";
                    ending.DominantNarration = "Europe becomes the world's economic powerhouse. Gleaming megacities stretch across the continent, every citizen a consumer, every corner a marketplace. The numbers are dazzling. You built a rich Europe.";
                    break;
                case WelfareIndicator.Gini:
                    ending.DominantTitle = "Equality Utopia";
                    ending.DominantNarration = "The gap closes. For the first time in modern history, a janitor's child and a CEO's child attend the same schools, visit the same hospitals, dream the same dreams. You chose fairness, and genuine social trust appeared.";
                    break;
                case WelfareIndicator.HumanCapital:
                    ending.DominantTitle = "Enlightened Society";
                    ending.DominantNarration = "Europe becomes the world's classroom. Universities overflow, libraries never close, and citizens debate philosophy as naturally as they discuss the weather. You invested in minds \u2014 and minds repaid you a thousandfold.";
                    break;
                case WelfareIndicator.LifeExpectancy:
                    ending.DominantTitle = "Golden Age of Health";
                    ending.DominantNarration = "Europeans live longer, healthier lives than any generation before them. Clean air, universal care, and preventive medicine have added years and quality to every life. You chose life, and life chose you back.";
                    break;
            }
        }
        else
        {
            switch (ending.DominantIndicator)
            {
                case WelfareIndicator.GDP:
                    ending.DominantTitle = "Economic Collapse";
                    ending.DominantNarration = "Factories close. Shops shutter. The great European project stumbles as economies contract and unemployment soars. Citizens line up for aid packages, wondering how the continent that once led the world now struggles to feed itself.";
                    break;
                case WelfareIndicator.Gini:
                    ending.DominantTitle = "Divided Europe";
                    ending.DominantNarration = "Two Europes emerge \u2014 one of penthouses and private jets, another of food banks and crumbling infrastructure. Your parliament chose growth over fairness, and now the cracks run deep.";
                    break;
                case WelfareIndicator.HumanCapital:
                    ending.DominantTitle = "Age of Ignorance";
                    ending.DominantNarration = "Misinformation spreads unchecked. Critical thinking fades as education crumbles and media literacy vanishes. Citizens vote on headlines they never read past. Your parliament let the foundations rot.";
                    break;
                case WelfareIndicator.LifeExpectancy:
                    ending.DominantTitle = "Public Health Crisis";
                    ending.DominantNarration = "Hospitals overflow. Life expectancy drops for the first time in a century. Pollution, underfunded healthcare, and environmental neglect take their toll. Your parliament's choices echo in every waiting room.";
                    break;
            }
        }

        return ending;
    }

    public void UpdateUI() {
        GameManager.Instance.indicators[(int)WelfareIndicator.GDP].text.text = $"{GDP}/{GDP_MAX}";
        GameManager.Instance.indicators[(int)WelfareIndicator.GDP].bar.localScale = new Vector3(NormalizedGDP, 1f, 1f);

        GameManager.Instance.indicators[(int)WelfareIndicator.Gini].text.text = $"{Gini}/{GINI_MAX}";
        GameManager.Instance.indicators[(int)WelfareIndicator.Gini].bar.localScale = new Vector3(NormalizedGini, 1f, 1f);

        GameManager.Instance.indicators[(int)WelfareIndicator.HumanCapital].text.text = $"{HumanCapital}/{HC_MAX}";
        GameManager.Instance.indicators[(int)WelfareIndicator.HumanCapital].bar.localScale = new Vector3(NormalizedHumanCapital, 1f, 1f);

        GameManager.Instance.indicators[(int)WelfareIndicator.LifeExpectancy].text.text = $"{LifeExpectancy}/{LE_MAX}";
        GameManager.Instance.indicators[(int)WelfareIndicator.LifeExpectancy].bar.localScale = new Vector3(NormalizedLifeExpectancy, 1f, 1f);
    }
}
