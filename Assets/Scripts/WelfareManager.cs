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
    }
    
    public float NormalizedGDP => (GDP - 8000f) / 5000f;
    public float NormalizedGini => 1f - Gini;
    public float NormalizedHumanCapital => HumanCapital / 10f;
    public float NormalizedLifeExpectancy => (LifeExpectancy - 65f) / 20f;

    public float CompositeWelfareScore =>
        (NormalizedGDP + NormalizedGini + NormalizedHumanCapital + NormalizedLifeExpectancy) / 4f;
}
