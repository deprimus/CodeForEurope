using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    [Foldout("References")] public CanvasGroup _canvasGroup;
    [Foldout("References")] public TextMeshProUGUI _title;

    public static GameEndManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowGameEnd()
    {
        _canvasGroup.DOFade(1, 0.5f).SetEase(Ease.InCubic).ChangeStartValue(0);
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        var titleText = PickTitle();
        _title.text = titleText;
    }

    private string PickTitle()
    {
        if (GameManager.Instance.TraditionalistPoints < Config.LossThreshold)
        {
            return "Loss due to traditionalist party underperformance";
        }
        else if (GameManager.Instance.LeftPoints < Config.LossThreshold)
        {
            return "Loss due to left party underperformance";
        }
        else if (GameManager.Instance.RightPoints < Config.LossThreshold)
        {
            return "Loss due to right party underperformance";
        }
        else if (GameManager.Instance.LibertarianPoints < Config.LossThreshold)
        {
            return "Loss due to libertarian party underperformance";
        }
        
        
        if (GameManager.Instance.TraditionalistPoints >= Config.DominanceThreshold)
        {
            return "Loss due to traditionalist party dominance";
        }
        else if (GameManager.Instance.LeftPoints >= Config.DominanceThreshold)
        {
            return "Loss due to left party dominance";
        }
        else if (GameManager.Instance.RightPoints >= Config.DominanceThreshold)
        {
            return "Loss due to right party dominance";
        }
        else if (GameManager.Instance.LibertarianPoints >= Config.DominanceThreshold)
        {
            return "Loss due to libertarian party dominance";
        }

        return "True equilibrium!";
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
