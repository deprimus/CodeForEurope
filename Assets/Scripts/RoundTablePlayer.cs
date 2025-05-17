using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
public class RoundTablePlayer : Faction
{
    [Foldout("References")] public UIView_MoodPicker _moodPicker;

    protected async override Task PickFactionMood()
    {
        var moodPicked = false;

        _moodPicker.Show(PickMood);

        await UniTask.WaitUntil(() => moodPicked);

        await UniTask.Delay(250);

        return;

        void PickMood(Mood mood)
        {
            moodPicked = true;
            SetMood(mood);
        }
    }
}
