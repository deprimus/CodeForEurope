using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-1000)]
public class SettingsMaster : MonoBehaviour
{
    public static SettingsMaster instance;

    [NonSerialized]
    public float musicVolume = 0.5f;

    [SerializeField]
    GameObject canvas;
    [SerializeField]
    Slider musicVolumeSlider;
    [SerializeField]
    GameObject returnToMenu;

    bool show;

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        show = false;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        Toggle();
    }

    public void Toggle()
    {
        bool menu = SceneManager.GetActiveScene().path.Contains("MainMenu");

        returnToMenu.SetActive(!menu);

        show = !show;
        canvas.SetActive(show);
    }

    public void Reset()
    {
        SetMusicVolume(0.5f);
    }

    public void SetMusicVolume(float vol)
    {
        musicVolumeSlider.value = vol;
        musicVolume = vol;

        Tale.Master.Props.audio.music.volume = musicVolume;
    }
}
