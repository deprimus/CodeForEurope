using UnityEngine;
using NaughtyAttributes;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class RoundTableManager : MonoBehaviour
{
    [Foldout("Components")] public UIView_Law _lawView;
    [Foldout("Components")] public Faction[] _people;

    public static RoundTableManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowLawCard()
    {
        var law = GameManager.Instance.CurrentLaw;
        _lawView.ShowLaw(law);
    }

    public async Task ShowMoodBars()
    {
        foreach (var person in _people)
        {
            await person.ShowMood();

            await UniTask.Delay(500);
        }

        await UniTask.Delay(3000);

        GameManager.Instance.OnMoodBarsHidden();
    }

    public void Influence(InteractionEffectType faction, int value)
    {
        switch (faction)
        {
            case InteractionEffectType.TraditionalistParty:
            case InteractionEffectType.ProgressistParty:
            case InteractionEffectType.LiberalParty:
            case InteractionEffectType.GreensParty:
            {
                var (primary, secondary) = GetFaction(faction);

                foreach (var person in _people)
                {
                    if (person.PrimaryOrientation == primary && person.SecondaryOrientation == secondary)
                    {
                        person.Influence(value);
                    }
                }

                break;
            }

            case InteractionEffectType.Lefts:
            case InteractionEffectType.Rights:
            case InteractionEffectType.Libertarians:
            case InteractionEffectType.Traditionalists:
            {
                var orientation = InteractionEffectToOrientation(faction);

                foreach (var person in _people)
                {
                    if (person.PrimaryOrientation == orientation || person.SecondaryOrientation == orientation)
                    {
                        person.Influence(value);
                    }
                }

                break;
            }
        }
    }

    private (FactionType primary, FactionType secondary) GetFaction(InteractionEffectType effect)
    {
        switch (effect)
        {
            case InteractionEffectType.TraditionalistParty:
                return (FactionType.Traditionalist, FactionType.Left);

            case InteractionEffectType.ProgressistParty:
                return (FactionType.Libertarian, FactionType.Right);

            case InteractionEffectType.LiberalParty:
                return (FactionType.Traditionalist, FactionType.Right);

            case InteractionEffectType.GreensParty:
                return (FactionType.Libertarian, FactionType.Left);

            default:
                return (FactionType.Traditionalist, FactionType.Traditionalist);
        }
    }

    private FactionType InteractionEffectToOrientation(InteractionEffectType effect)
    {
        switch (effect)
        {
            case InteractionEffectType.Lefts:
                return FactionType.Left;

            case InteractionEffectType.Rights:
                return FactionType.Right;

            case InteractionEffectType.Libertarians:
                return FactionType.Libertarian;

            case InteractionEffectType.Traditionalists:
                return FactionType.Traditionalist;

            default:
                return FactionType.Traditionalist;
        }
    }
}
