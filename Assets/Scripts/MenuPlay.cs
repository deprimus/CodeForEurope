using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPlay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public static bool disable;

    [NonSerialized]
    TextMeshProUGUI text;

    void Awake()
    {
        disable = true;
        text = GetComponent<TextMeshProUGUI>();

        Tale.Music.Play("Background", Tale.Music.PlayMode.LOOP, 0.7f);
        Tale.MagicFix();
        Transition.SweepIn();
        Tale.Exec(() => disable = false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (disable)
        {
            return;
        }

        disable = true;

        SoundManager.instance.Play(SoundManager.instance.select);

        Transition.SweepOut();

        Tale.Wait();
        Tale.Scene();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (disable)
        {
            return;
        }

        SoundManager.instance.Play(SoundManager.instance.flip);

        text.color = new Color(1f, 1f, 0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (disable)
        {
            return;
        }

        text.color = new Color(1f, 1f, 1f);
    }
}
