using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "LawManager", menuName = "Game/LawManager")]
public class LawManager : ScriptableObject
{
    [Foldout("References")] public GameData _gameData;

    public List<LawEffect> CurrentLawEffects => _currentLawEffects;

    public event Action OnLawEffectsChanged;

    private List<Law> _laws;
    private List<LawEffect> _currentLawEffects;
    public void Initialize()
    {
        _laws = _gameData.Laws.ToList();
        _currentLawEffects = new List<LawEffect>();
    }

    public void SetCurrentLawEffects(List<LawEffect> effects)
    {
        _currentLawEffects = effects;
        OnLawEffectsChanged?.Invoke();
    }

    public Law PickLaw()
    {
        var law = _laws[UnityEngine.Random.Range(0, _laws.Count)];

        // COM: delete this debugging code
        if (_laws.Count == 3)
        {
            law = _laws[2];
        }
        _laws.Remove(law);
        return law;
    }
}