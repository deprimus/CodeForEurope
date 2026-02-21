using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CloseSettings : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
        OnPointerExit(null);

        SettingsMaster.instance.Toggle();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.Play(SoundManager.instance.flip);

        text.color = new Color(1f, 1f, 0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = new Color(1f, 1f, 1f);
    }
}
