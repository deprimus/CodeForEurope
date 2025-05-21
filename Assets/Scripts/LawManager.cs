// -----------------------------------------------------------------------------
// LawManager.cs
//
// ScriptableObject for managing all laws and their effects in the game.
// Handles initialization, selection, and updating of laws and their effects on factions.
//
// Main Functions:
// - Initialize(): Loads laws from game data.
// - SetCurrentLawEffects(): Sets and broadcasts the current law's effects.
// - PickLaw(): Randomly selects a law for the current round.
//
// Fields:
// - _gameData: Reference to the main game data asset.
// - CurrentLawEffects: List of effects for the current law.
// -----------------------------------------------------------------------------

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

        _laws.Remove(law);
        return law;
    }
}