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
    public List<NPCInteraction> NPCInteractions;
}

[System.Serializable]
public class LawEffect
{
    public FactionType Type;
    public int Value;
}

[System.Serializable]
public enum FactionType
{
    Traditionalist,
    Left,
    Right,
    Libertarian
}
