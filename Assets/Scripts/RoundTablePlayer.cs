using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
public class RoundTablePlayer : Faction
{
    [Foldout("References")] public UIView_MoodPicker _moodPicker;
    [Foldout("References")] public UIView_VotePicker _votePicker;
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

    protected async override Task PickFactionVote()
    {
        var votePicked = false;

        _votePicker.Show(PickVote);

        await UniTask.WaitUntil(() => votePicked);

        await UniTask.Delay(250);

        return;

        void PickVote(bool vote)
        {
            votePicked = true;
            Vote = vote ? 1 : 0;
        }
    }
}
