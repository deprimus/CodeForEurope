using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
[CreateAssetMenu(fileName = "LawManager", menuName = "Game/LawManager")]
public class LawManager : ScriptableObject
{
    [Foldout("References")] public GameData _gameData;

    private List<Law> _laws;

    public void Initialize()
    {
        _laws = _gameData.Laws.ToList();
    }

    public Law PickLaw()
    {
        var law = _laws[Random.Range(0, _laws.Count)];
        _laws.Remove(law);
        return law;
    }
}