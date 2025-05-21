using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuQuit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{


    [NonSerialized]
    TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (MenuPlay.disable)
        {
            return;
        }

        MenuPlay.disable = true;

        SoundManager.instance.Play(SoundManager.instance.select);

        Transition.SweepOut();

        Tale.Wait();
        Application.Quit();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (MenuPlay.disable)
        {
            return;
        }

        SoundManager.instance.Play(SoundManager.instance.flip);

        text.color = new Color(1f, 1f, 0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (MenuPlay.disable)
        {
            return;
        }

        text.color = new Color(1f, 1f, 1f);
    }
}
