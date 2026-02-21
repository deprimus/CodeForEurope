// -----------------------------------------------------------------------------
// GameEndManager.cs
//
// Handles the end-of-game sequence, displaying the appropriate ending based on player choices and faction outcomes.
// Manages the end screen UI and transitions.
//
// Main Functions:
// - ShowGameEnd(): Displays the game end screen and triggers transitions.
//
// Fields:
// - _canvasGroup: UI group for fade-in/out.
// - _ending: Image for the ending illustration.
// - traditionalistEnd, leftEnd, rightEnd, libertarianEnd, harmonyEnd: Sprites for each ending.
// -----------------------------------------------------------------------------

using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameEndManager : MonoBehaviour
{
    [Foldout("References")] public CanvasGroup _canvasGroup;
    [Foldout("References")] public Image _ending;

    [Foldout("References")] public Sprite traditionalistEnd;
    [Foldout("References")] public Sprite leftEnd;
    [Foldout("References")] public Sprite rightEnd;
    [Foldout("References")] public Sprite libertarianEnd;
    [Foldout("References")] public Sprite harmonyEnd;

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
        //_canvasGroup.DOFade(1, 0.5f).SetEase(Ease.InCubic).ChangeStartValue(0);
        //_canvasGroup.blocksRaycasts = true;
        //_canvasGroup.interactable = true;

        Transition.SweepIn();
        Tale.Advance();
        Transition.SweepOut();

        Tale.Multiplex(
            Tale.Wait(),
            Tale.Music.Stop()
        );

        Tale.Scene("MainMenu");

        var ending = PickEnding();

        switch (ending) {
            case Ending.Traditionalist:
            {
                _ending.sprite = traditionalistEnd;
                break;
            }
            case Ending.Left:
            {
                _ending.sprite = leftEnd;
                break;
            }
            case Ending.Right:
            {
                _ending.sprite = rightEnd;
                break;
            }
            case Ending.Libertarian:
            {
                _ending.sprite = libertarianEnd;
                break;
            }
            case Ending.Harmony:
            {
                _ending.sprite = harmonyEnd;
                break;
            }
        }
    }

    private Ending PickEnding()
    {        
        if (GameManager.Instance.TraditionalistPoints >= Config.DominanceThreshold)
        {
            return Ending.Traditionalist;
        }
        else if (GameManager.Instance.LeftPoints >= Config.DominanceThreshold)
        {
            return Ending.Left;
        }
        else if (GameManager.Instance.RightPoints >= Config.DominanceThreshold)
        {
            return Ending.Right;
        }
        else if (GameManager.Instance.LibertarianPoints >= Config.DominanceThreshold)
        {
            return Ending.Libertarian;
        }

        return Ending.Harmony;
    }
}
