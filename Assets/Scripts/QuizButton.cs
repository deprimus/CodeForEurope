// -----------------------------------------------------------------------------
// QuizButton.cs
// Attach to an Image (or parent of a TextMeshProUGUI). Handles hover/click and
// forwards the chosen QuizAnswer to QuizManager.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class QuizButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] QuizManager manager;
    [SerializeField] QuizAnswer answer;

    TextMeshProUGUI text;

    static readonly Color HoverColor = new Color(255, 216, 0);
    static readonly Color NormalColor = Color.white;

    void Awake() {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (text != null) {
            text.color = HoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (text != null) {
            text.color = NormalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (manager != null) {
            manager.Choose(answer);
        }
    }
}
