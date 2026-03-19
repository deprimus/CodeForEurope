using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUDPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    GameObject popup;

    Image image;
    bool shouldHideOnNextClick;
    int openedFrame;

    void Awake() {
        image = GetComponent<Image>();
        SetAlpha(0.9f);
    }

    void Update() {
        if (!shouldHideOnNextClick) {
            return;
        }

        if (Time.frameCount == openedFrame) {
            return;
        }

        if (UnityEngine.Input.GetMouseButtonDown(0)) {
            popup.SetActive(false);
            shouldHideOnNextClick = false;
            TaleUtil.Input.Release();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        SetAlpha(1f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        SetAlpha(0.9f);
    }

    public void OnPointerClick(PointerEventData eventData) {
        TaleUtil.Input.Hijack();
        popup.SetActive(true);
        shouldHideOnNextClick = true;
        openedFrame = Time.frameCount;
    }

    void SetAlpha(float alpha) {
        if (image == null) {
            return;
        }

        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
