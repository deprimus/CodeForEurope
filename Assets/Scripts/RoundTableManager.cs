// -----------------------------------------------------------------------------
// RoundTableManager.cs
//
// Controls the round table scene, showing law cards, managing faction moods, and handling voting and influence mechanics.
// Coordinates with GameManager and LawManager to update the game state based on player and NPC actions.
//
// Main Functions:
// - ShowLawCard(): Displays the current law card in the UI.
// - ShowMoodBars(): Animates and displays faction mood bars.
// - Influence(): Applies influence to a faction based on interaction effects.
// - VoteLaw(): Handles the law voting process.
//
// Fields:
// - _lawView: UI component for law display.
// - _people: Array of faction representatives.
// - _lawApproved, _lawRejected: UI elements for law outcomes.
// -----------------------------------------------------------------------------

using UnityEngine;
using NaughtyAttributes;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Linq;
using DG.Tweening;
public class RoundTableManager : MonoBehaviour
{
    [Foldout("Components")] public UIView_Law _lawView;
    [Foldout("Components")] public Faction[] _people;
    [Foldout("References")] public CanvasGroup _lawApproved;
    [Foldout("References")] public CanvasGroup _lawRejected;

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
            var mood = await person.ShowMood();

            if (person is not RoundTablePlayer)
            {
                AudioClip clip = null;

                switch (mood)
                {
                    case Mood.Neutral:
                    {
                        clip = SoundManager.instance.hmm[Random.Range(0, SoundManager.instance.hmm.Length)];
                        break;
                    }
                    case Mood.Happy:
                    {
                        clip = SoundManager.instance.yes[Random.Range(0, SoundManager.instance.yes.Length)];
                        break;
                    }
                    case Mood.Angry:
                    {
                        clip = SoundManager.instance.no[Random.Range(0, SoundManager.instance.no.Length)];
                        break;
                    }
                }

                SoundManager.instance.Play(clip);
            }

            await UniTask.Delay(1500);
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

    public async void VoteLaw()
    {
        await UniTask.Delay(2000);

        foreach (var person in _people)
        {
            await person.ShowVote();

            SoundManager.instance.Play(SoundManager.instance.appear2);

            await UniTask.Delay(2000);
        }

        var lawApproved = _people.Sum(p => p.Vote) / _people.Length > 0.5f;

        var lawObject = lawApproved ? _lawApproved : _lawRejected;

        lawObject.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InCubic).ChangeStartValue(Vector3.one * 1.2f);
        lawObject.DOFade(1, 0.5f).SetEase(Ease.InCubic).ChangeStartValue(0);

        await UniTask.Delay(2000);

        lawObject.transform.DOScale(Vector3.one * 0.8f, 0.5f).SetEase(Ease.InCubic);
        lawObject.DOFade(0, 0.5f).SetEase(Ease.InCubic);

        await UniTask.Delay(1000);

        GameManager.Instance.OnVoteEnded(lawApproved);
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
