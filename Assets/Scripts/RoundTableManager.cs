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
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
public class RoundTableManager : MonoBehaviour
{
    [Foldout("References")] public Animator _cameraAnimator;
    [Foldout("Components")] public UIView_Law _lawView;
    [Foldout("Components")] public Faction[] _people;
    [Foldout("References")] public CanvasGroup _lawApproved;
    [Foldout("References")] public CanvasGroup _lawRejected;
    [Foldout("References")] public RectTransform _effects;
    [Foldout("References")] public TextMeshProUGUI _effectText;
    [Foldout("References")] public UIView_Law _effectLaw;
    [Foldout("References")] public GameObject _effectApproved;
    [Foldout("References")] public GameObject _effectRejected;
    [Foldout("References")] public TextMeshProUGUI _effectInfluencesText;

    public static RoundTableManager Instance { get; private set; }

    private RoundTablePlayer _user;

    private void Awake()
    {
        Instance = this;

        _user = _people.FirstOrDefault(p => p is RoundTablePlayer) as RoundTablePlayer;

        _effects.transform.localScale = Vector3.zero;
    }

    public void SetCameraAnimation(string animation)
    {
        _cameraAnimator.Play(animation);
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

                SoundManager.instance.Play(clip, 0.7f);
            }

            await UniTask.Delay(1500);
        }

        await UniTask.Delay(2000);

        GameManager.Instance.OnMoodBarsHidden();
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

        var userLawApprovalStatus = _user.UserLawApprovalStatus;

        var random = Random.Range(0, 101);
        var userWinPercentage = GameManager.BaseUserWinPercentage;
        var userLawInfluence = GameManager.Instance.UserLawInfluence;

        //userWinPercentage += ((userLawApprovalStatus ^ (userLawInfluence > 0)) * (-2) + 1) * Mathf.Abs(userLawInfluence);
        userWinPercentage += userLawApprovalStatus ? userLawInfluence : -userLawInfluence;
        
        var lawApproved = random <= userWinPercentage ? userLawApprovalStatus : !userLawApprovalStatus;

        //lawObject.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InCubic).ChangeStartValue(Vector3.one * 1.2f);
        //lawObject.DOFade(1, 0.5f).SetEase(Ease.InCubic).ChangeStartValue(0);

        //await UniTask.Delay(2000);

        //lawObject.transform.DOScale(Vector3.one * 0.8f, 0.5f).SetEase(Ease.InCubic);
        //lawObject.DOFade(0, 0.5f).SetEase(Ease.InCubic);

        _effectLaw._title.text = _lawView._title.text;
        _effectLaw._description.text = _lawView._description.text;
        _effectLaw._effect.text = _lawView._effect.text;

        _effectApproved.SetActive(lawApproved);
        _effectRejected.SetActive(!lawApproved);

        var welfareEffects = GameManager.Instance.CurrentLaw?.WelfareEffects;
        float multiplier = lawApproved ? 1f : -1f;

        float? GetEffectValue(WelfareIndicator indicator)
        {
            var e = welfareEffects?.FirstOrDefault(x => x.Indicator == indicator);
            return e != null ? e.Value : (float?)null;
        }

        string FormatSigned(float value) =>
            value >= 0f ? $"+<color=green>{value:0.##}</color>" : $"<color=red>{value:0.##}</color>";

        string FormatLine(WelfareIndicator indicator, string name)
        {
            var v = GetEffectValue(indicator);
            if (!v.HasValue || Mathf.Abs(v.Value) < 0.00001f)
                return "<color=grey>---</color>";

            var signedValue = v.Value * multiplier;
            return $"{FormatSigned(signedValue)} {name}";
        }

        var lines = new List<string>(4)
        {
            FormatLine(WelfareIndicator.GDP, "GDP Per Capita"),
            FormatLine(WelfareIndicator.Gini, "Gini Index"),
            FormatLine(WelfareIndicator.HumanCapital, "Human Capital"),
            FormatLine(WelfareIndicator.LifeExpectancy, "Life Expectancy")
        };

        var body = string.Join("\n", lines);
        _effectText.text = body;

        var lawEffects = GameManager.Instance.CurrentLaw?.Effects;

        int SumInfluence(FactionType type)
        {
            if (lawEffects == null) return 0;

            int sum = 0;
            foreach (var e in lawEffects)
                if (e.Type == type)
                    sum += e.Value;

            return sum;
        }

        string FormatInfluenceLine(int value, string colorHex, string name)
        {
            if (!lawApproved || Mathf.Abs(value) < 0.00001f)
                return "<color=grey>---</color>";

            var signed = value >= 0 ? $"+{value}" : value.ToString();
            return $"<color={colorHex}>{signed} {name}</color>";
        }

        var tValue = SumInfluence(FactionType.Traditionalist);
        var gValue = SumInfluence(FactionType.Left);
        var lValue = SumInfluence(FactionType.Libertarian);
        var pValue = SumInfluence(FactionType.Right);

        var influenceLines = string.Join("\n",
            FormatInfluenceLine(tValue, "#FF3443", "Traditionalists"),
            FormatInfluenceLine(gValue, "#42FF42", "Greens"),
            FormatInfluenceLine(lValue, "#7FC9FF", "Liberals"),
            FormatInfluenceLine(pValue, "#FFD800", "Progresists")
        );

        _effectInfluencesText.text = influenceLines;

        _effects.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.InCubic).ChangeStartValue(Vector3.zero);

        await UniTask.Delay(500);

        Tale.Async(
            Tale.Queue(
                Tale.Advance(),
                Tale.Exec(() =>
                {
                    _effects.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InCubic);
                    GameManager.Instance.OnVoteEnded(lawApproved);
                })
            )
        );
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
            case InteractionEffectType.AllLefts:
                return FactionType.Left;

            case InteractionEffectType.AllRights:
                return FactionType.Right;

            case InteractionEffectType.AllLibertarians:
                return FactionType.Libertarian;

            case InteractionEffectType.AllTraditionalists:
                return FactionType.Traditionalist;

            default:
                return FactionType.Traditionalist;
        }
    }
}
