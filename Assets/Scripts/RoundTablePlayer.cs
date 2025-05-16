using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
public class RoundTablePlayer : Person
{
    [Foldout("References")] public UIView_MoodPicker _moodPicker;

    public async override Task ShowMood()
    {
        var moodPicked = false;

        _moodPicker.Show(PickMood);

        await UniTask.WaitUntil(() => moodPicked);

        await UniTask.Delay(250);

        await base.ShowMood();

        return;

        void PickMood(Mood mood)
        {
            moodPicked = true;
            SetMood(mood);
        }
    }
}
