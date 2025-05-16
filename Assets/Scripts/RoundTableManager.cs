using UnityEngine;
using NaughtyAttributes;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class RoundTableManager : MonoBehaviour
{
    [Foldout("Components")] public UIView_Law _lawView;
    [Foldout("Components")] public Person[] _people;

    public static RoundTableManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowLawCard()
    {
        var law = GameManager.Instance.CurrentLaw;
        _lawView.ShowLaw(law);
    }

    public async Task ShowMoodBars()
    {
        foreach (var person in _people)
        {
            await person.ShowMood();

            await UniTask.Delay(500);
        }

        await UniTask.Delay(3000);

        GameManager.Instance.OnMoodBarsHidden();
    }
}
