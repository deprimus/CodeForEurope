using UnityEngine;
using NaughtyAttributes;
using System.Threading.Tasks;

public class Faction : MonoBehaviour
{
    [Foldout("Components")] public UIView_Mood _moodView;

    private Mood _mood;

    private void Awake()
    {
        SetMood((Mood)Random.Range(0, System.Enum.GetValues(typeof(Mood)).Length));
    }

    public async virtual Task ShowMood()
    {
        _moodView.ShowMood(_mood);
    }

    protected void SetMood(Mood mood)
    {
        _mood = mood;
    }
}
