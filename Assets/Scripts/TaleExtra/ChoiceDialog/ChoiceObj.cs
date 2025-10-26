using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace TaleUtil.Scripts.Choice.Dialog {
    public class ChoiceObj : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        [SerializeField]
        ChoiceMaster master;

        [SerializeField]
        Image img;
        [SerializeField]
        TextMeshProUGUI text;
        [SerializeField]
        Image highlight;

        TaleUtil.Delegates.ShallowDelegate action;

        internal void Present(string choice, Sprite sprite, TaleUtil.Delegates.ShallowDelegate onChoice) {
            text.text = choice;
            action = onChoice;
            img.sprite = sprite;
            highlight.color = new Color(highlight.color.r, highlight.color.g, highlight.color.b, 0f);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!master.IsChoosing()) {
                return;
            }

            SoundManager.instance.Play(SoundManager.instance.flip);

            text.color = Color.black;
            highlight.color = new Color(highlight.color.r, highlight.color.g, highlight.color.b, 1f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!master.IsChoosing()) {
                return;
            }

            text.color = Color.white;
            highlight.color = new Color(highlight.color.r, highlight.color.g, highlight.color.b, 0f);
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!master.IsChoosing()) {
                return;
            }

            SoundManager.instance.Play(SoundManager.instance.select);

            OnPointerExit(null);

            action();
        }
    }
}