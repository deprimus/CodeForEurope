using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
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
}

[System.Serializable]
public class LawEffect
{
    public LawEffectType Type;
    public int Value;
}

[System.Serializable]
public enum LawEffectType
{
    Traditionalist,
    Left,
    Right,
    Libertarian
}