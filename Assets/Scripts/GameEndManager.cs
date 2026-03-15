using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class GameEndManager : MonoBehaviour
{
    [Foldout("References")] public CanvasGroup _canvasGroup;

    [Foldout("Faction UI")] public Image _factionImage;
    [Foldout("Faction UI")] public TextMeshProUGUI _factionText;

    [Foldout("Welfare Tier UI")] public TextMeshProUGUI _welfareScoreText;
    [Foldout("Welfare Tier UI")] public TextMeshProUGUI _welfareTierText;
    [Foldout("Welfare Tier UI")] public TextMeshProUGUI _welfareTierNarrationText;

    [Foldout("Welfare Dominant UI")] public TextMeshProUGUI _welfareDominantText;
    [Foldout("Welfare Dominant UI")] public TextMeshProUGUI _welfareDominantNarrationText;

    [Foldout("Faction Endings")] public Sprite traditionalistEnd;
    [Foldout("Faction Endings")] public Sprite leftEnd;
    [Foldout("Faction Endings")] public Sprite rightEnd;
    [Foldout("Faction Endings")] public Sprite libertarianEnd;
    [Foldout("Faction Endings")] public Sprite harmonyEnd;

    public enum Ending
    {
        Traditionalist,
        Left,
        Right,
        Libertarian,
        Harmony
    }

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

        var factionEnding = PickEnding();
        var welfare = GameManager.Instance.Welfare;
        var we = welfare.GetEnding();

        // Faction
        if (_factionImage != null)
            _factionImage.sprite = GetFactionSprite(factionEnding);
        if (_factionText != null)
            _factionText.text = factionEnding.ToString();

        // Welfare tier
        if (_welfareScoreText != null)
            _welfareScoreText.text = $"Welfare Score: {welfare.CompositeWelfareScore:P0}";
        if (_welfareTierText != null)
            _welfareTierText.text = we.TierTitle;
        if (_welfareTierNarrationText != null)
            _welfareTierNarrationText.text = we.TierNarration;

        // Welfare dominant
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

    private Sprite GetFactionSprite(Ending ending) => ending switch
    {
        Ending.Traditionalist => traditionalistEnd,
        Ending.Left           => leftEnd,
        Ending.Right          => rightEnd,
        Ending.Libertarian    => libertarianEnd,
        Ending.Harmony        => harmonyEnd,
        _ => harmonyEnd
    };

    private Ending PickEnding()
    {
        if (GameManager.Instance.TraditionalistPoints >= Config.DominanceThreshold)
            return Ending.Traditionalist;
        else if (GameManager.Instance.LeftPoints >= Config.DominanceThreshold)
            return Ending.Left;
        else if (GameManager.Instance.RightPoints >= Config.DominanceThreshold)
            return Ending.Right;
        else if (GameManager.Instance.LibertarianPoints >= Config.DominanceThreshold)
            return Ending.Libertarian;

        return Ending.Harmony;
    }
}
