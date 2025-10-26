using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetSettings : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [NonSerialized]
    TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.instance.Play(SoundManager.instance.select);

        SettingsMaster.instance.Reset();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.Play(SoundManager.instance.flip);
        OnPointerExit(null);

        text.color = new Color(1f, 1f, 0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = new Color(1f, 1f, 1f);
    }
}
