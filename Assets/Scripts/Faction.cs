using UnityEngine;
using NaughtyAttributes;
using System.Threading.Tasks;
using System.Linq;
public class Faction : MonoBehaviour
{
    [Foldout("Components")] public FactionType _primaryOrientation;
    [Foldout("Components")] public FactionType _secondaryOrientation;
    [Foldout("Components")] public UIView_Mood _moodView;

    private int _moodValue;
    private Mood _mood;

    public async virtual Task ShowMood()
    {
        await PickFactionMood();

        _moodView.ShowMood(_mood);
    }

    protected void SetMood(Mood mood)
    {
        _mood = mood;
    }

    protected virtual async Task PickFactionMood()
    {
        var law = GameManager.Instance.CurrentLaw;
        if (law == null)
        {
            SetMood(Mood.Neutral);
            return;
        }

        var favoringPrimaryEffect = law.Effects.Find(e => e.Type == _primaryOrientation);
        var favoringSecondaryEffect = law.Effects.Find(e => e.Type == _secondaryOrientation);
        var opposingPrimaryEffect = law.Effects.Find(e => e.Type == GetOpposingFaction(_primaryOrientation));
        var opposingSecondaryEffect = law.Effects.Find(e => e.Type == GetOpposingFaction(_secondaryOrientation));

        var cap = law.Effects.Sum(e => e.Value);

        float score = 0;
        score += favoringPrimaryEffect?.Value ?? 0;
        score += favoringSecondaryEffect?.Value ?? 0;
        score -= opposingPrimaryEffect?.Value ?? 0;
        score -= opposingSecondaryEffect?.Value ?? 0;

        _moodValue = (int)score.Map(-cap, cap, 0, 100);

        switch (_moodValue)
        {
            case >= 70:
                SetMood(Mood.Happy);
                break;

            case >= 40:
                SetMood(Mood.Neutral);
                break;

            default:
                SetMood(Mood.Angry);
                break;
        }
    }

    private FactionType GetOpposingFaction(FactionType faction)
    {
        switch (faction)
        {
            case FactionType.Traditionalist:
                return FactionType.Libertarian;

            case FactionType.Libertarian:
                return FactionType.Traditionalist;

            case FactionType.Left:
                return FactionType.Right;

            case FactionType.Right:
                return FactionType.Left;

            default:
                return FactionType.Traditionalist;
        }
    }
}
