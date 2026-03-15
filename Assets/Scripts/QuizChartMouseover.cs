// -----------------------------------------------------------------------------
// QuizChartMouseover.cs
// On pointer enter/exit, activates/deactivates a target GameObject.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

public class QuizChartMouseover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] GameObject target;

    public void OnPointerEnter(PointerEventData eventData) {
        if (target != null) {
            target.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (target != null) {
            target.SetActive(false);
        }
    }
}
