using System;
using System.Collections.Generic;
using System.Linq;

public class LawManager
{
    public List<LawEffect> CurrentLawEffects => _currentLawEffects;

    public event Action OnLawEffectsChanged;

    private List<Law> _laws;
    private List<LawEffect> _currentLawEffects;

    public LawManager(List<Law> laws)
    {
        _laws = laws.ToList();
        _currentLawEffects = new List<LawEffect>();
    }

    public void Initialize()
    {
        _laws = GameDatabase.Instance.Laws.ToList();
        _currentLawEffects = new List<LawEffect>();
    }

    public void SetCurrentLawEffects(List<LawEffect> effects)
    {
        _currentLawEffects = effects;
        OnLawEffectsChanged?.Invoke();
    }

    public Law PickLaw()
    {
        if (_laws.Count == 0) return null;

        var law = _laws[UnityEngine.Random.Range(0, _laws.Count)];
        _laws.Remove(law);
        return law;
    }
}
