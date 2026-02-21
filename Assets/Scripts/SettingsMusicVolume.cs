using UnityEngine;
using UnityEngine.UI;

public class SettingsMusicVolume : MonoBehaviour
{
    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void UpdateValue()
    {
        SettingsMaster.instance.SetMusicVolume(slider.value);
    }
}
