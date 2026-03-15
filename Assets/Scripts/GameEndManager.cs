using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class GameEndManager : MonoBehaviour
{
    [Foldout("References")] public CanvasGroup _canvasGroup;
    [Foldout("References")] public Image _ending;

    [Foldout("References")] public Sprite traditionalistEnd;
    [Foldout("References")] public Sprite leftEnd;
    [Foldout("References")] public Sprite rightEnd;
    [Foldout("References")] public Sprite libertarianEnd;
    [Foldout("References")] public Sprite harmonyEnd;

    [Foldout("Welfare UI")] public TextMeshProUGUI _welfareScoreText;
    [Foldout("Welfare UI")] public TextMeshProUGUI _welfareTierText;
    [Foldout("Welfare UI")] public TextMeshProUGUI _welfareDominantText;
    [Foldout("Welfare UI")] public TextMeshProUGUI _welfareNarrationText;

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
        Transition.SweepIn();
        Tale.Advance();
        Transition.SweepOut();

        Tale.Multiplex(
            Tale.Wait(),
            Tale.Music.Stop()
        );

        Tale.Scene("MainMenu");

        // Faction ending (visual)
        var ending = PickEnding();
        switch (ending)
        {
            case Ending.Traditionalist: _ending.sprite = traditionalistEnd; break;
            case Ending.Left:           _ending.sprite = leftEnd; break;
            case Ending.Right:          _ending.sprite = rightEnd; break;
            case Ending.Libertarian:    _ending.sprite = libertarianEnd; break;
            case Ending.Harmony:        _ending.sprite = harmonyEnd; break;
        }

        // Welfare ending (text)
        var welfare = GameManager.Instance.Welfare;
        var we = welfare.GetEnding();

        if (_welfareScoreText != null)
            _welfareScoreText.text = $"Welfare Score: {welfare.CompositeWelfareScore:P0}";
        if (_welfareTierText != null)
            _welfareTierText.text = we.TierTitle;
        if (_welfareDominantText != null)
            _welfareDominantText.text = we.DominantTitle;
        if (_welfareNarrationText != null)
            _welfareNarrationText.text = $"{we.TierNarration}\n\n{we.DominantNarration}";
    }

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
