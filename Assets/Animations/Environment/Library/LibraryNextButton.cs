using UnityEngine;

public class LibraryNextButton : MonoBehaviour
{
    public static LibraryNextButton Instance { get; private set; }

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        Instance = this;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        Hide();
    }

    public void Show()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public void OnClick()
    {
        GameManager.Instance.OnLibraryEnded();
    }
}
