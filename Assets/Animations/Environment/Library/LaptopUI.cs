using UnityEngine;
using DG.Tweening;
using MEC;
using UnityEngine.UI;

public class LaptopUI : MonoBehaviour
{
    [SerializeField] private RectTransform _contentTransform;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutCubic);
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        Timing.CallDelayed(Timing.WaitForOneFrame, () => {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentTransform);
        });
    }

    public void Hide()
    {
        LibraryCamera.Instance.RestoreCamera();
        _canvasGroup.DOFade(0, 0.5f).SetEase(Ease.InCubic)
            .OnComplete(() => {
                gameObject.SetActive(false);
            });
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }
}
