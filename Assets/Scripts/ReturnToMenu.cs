using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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

        SettingsMaster.instance.Toggle();

        // TODO: this is a hack and doesn't get rid of the old props which still exist
        DestroyImmediate(GameObject.FindAnyObjectByType<TaleMaster>());
        Tale.alive = false;
        TaleUtil.Props.audio.music.Stop();
        SceneManager.LoadScene("Scenes/MainMenu");
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
