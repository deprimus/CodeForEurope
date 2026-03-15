using System.Collections.Generic;
using UnityEngine;

public class GameData : ScriptableObject
{
    public List<Law> Laws;
}

[System.Serializable]
public class Law
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public List<LawEffect> Effects;
    public List<WelfareEffect> WelfareEffects;
    public List<NPCInteraction> NPCInteractions;
}

[System.Serializable]
public class LawEffect
{
    public FactionType Type;
    public int Value;
}

[System.Serializable]
public class WelfareEffect
{
    public WelfareIndicator Indicator;
    public float Value;
}

[System.Serializable]
public enum FactionType
{
    Traditionalist,
    Left,
    Right,
    Libertarian
}

[System.Serializable]
public enum WelfareIndicator
{
    GDP,
    Gini,
    HumanCapital,
    LifeExpectancy
}
