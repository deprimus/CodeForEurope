using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class GameEndManager : MonoBehaviour
{
    [Foldout("References")] public CanvasGroup _canvasGroup;

    [Foldout("Welfare Tier UI")] public TextMeshProUGUI _welfareScoreText;
    [Foldout("Welfare Tier UI")] public TextMeshProUGUI _welfareTierText;
    [Foldout("Welfare Tier UI")] public TextMeshProUGUI _welfareTierNarrationText;

    [Foldout("Welfare Dominant UI")] public Image _welfareDominantImage;
    [Foldout("Welfare Dominant UI")] public TextMeshProUGUI _welfareDominantText;
    [Foldout("Welfare Dominant UI")] public TextMeshProUGUI _welfareDominantNarrationText;

    [Foldout("Welfare Dominant Sprites")] public Sprite gdpGoodSprite;
    [Foldout("Welfare Dominant Sprites")] public Sprite giniGoodSprite;
    [Foldout("Welfare Dominant Sprites")] public Sprite humanCapitalGoodSprite;
    [Foldout("Welfare Dominant Sprites")] public Sprite lifeExpectancyGoodSprite;

    [Foldout("Welfare Dominant Sprites")] public Sprite gdpBadSprite;
    [Foldout("Welfare Dominant Sprites")] public Sprite giniBadSprite;
    [Foldout("Welfare Dominant Sprites")] public Sprite humanCapitalBadSprite;
    [Foldout("Welfare Dominant Sprites")] public Sprite lifeExpectancyBadSprite;

    public static GameEndManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowGameEnd()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        var welfare = GameManager.Instance.Welfare;
        var we = welfare.GetEnding();

        // Welfare tier
        if (_welfareScoreText != null)
            _welfareScoreText.text = $"Welfare Score: {welfare.CompositeWelfareScore:P0}";
        if (_welfareTierText != null)
            _welfareTierText.text = we.TierTitle;
        if (_welfareTierNarrationText != null)
            _welfareTierNarrationText.text = we.TierNarration;

        // Welfare dominant
        if (_welfareDominantImage != null)
            _welfareDominantImage.sprite = GetWelfareDominantSprite(we);
        if (_welfareDominantText != null)
            _welfareDominantText.text = we.DominantTitle;
        if (_welfareDominantNarrationText != null)
            _welfareDominantNarrationText.text = we.DominantNarration;

        Transition.SweepIn();
        Tale.Advance();
        Transition.SweepOut();

        Tale.Multiplex(
            Tale.Wait(),
            Tale.Music.Stop()
        );

        Tale.Scene("MainMenu");
    }

    private Sprite GetWelfareDominantSprite(WelfareManager.WelfareEnding we)
    {
        bool isGood = we.Tier == WelfareManager.CompositeTier.Flourishing;

        return (we.DominantIndicator, isGood) switch
        {
            (WelfareIndicator.GDP, true) => gdpGoodSprite,
            (WelfareIndicator.Gini, true) => giniGoodSprite,
            (WelfareIndicator.HumanCapital, true) => humanCapitalGoodSprite,
            (WelfareIndicator.LifeExpectancy, true) => lifeExpectancyGoodSprite,
            (WelfareIndicator.GDP, false) => gdpBadSprite,
            (WelfareIndicator.Gini, false) => giniBadSprite,
            (WelfareIndicator.HumanCapital, false) => humanCapitalBadSprite,
            (WelfareIndicator.LifeExpectancy, false) => lifeExpectancyBadSprite,
            _ => null
        };
    }
}