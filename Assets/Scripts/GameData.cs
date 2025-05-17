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
}
