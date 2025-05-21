using UnityEngine;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
public class GameManager : MonoBehaviour
{
    [Foldout("References")] public LawManager _lawManager;
    [Foldout("References")] public NPCManager _npcManager;

    public int TraditionalistPoints => _traditionalistPoints;
    public int LeftPoints => _leftPoints;
    public int RightPoints => _rightPoints;
    public int LibertarianPoints => _libertarianPoints;
    
    public static GameManager Instance { get; private set; }

    public Law CurrentLaw { get; private set; }

    private int _roundIndex = 0;

    [Foldout("Debug"), SerializeField, ReadOnly]
    private int _traditionalistPoints;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private int _leftPoints;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private int _rightPoints;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private int _libertarianPoints;

    private async void Awake()
    {
        Instance = this;

        await UniTask.Delay(500);

        StartGame();
    }

    public void StartGame()
    {
        _roundIndex = 0;

        _lawManager.Initialize();

        //Transition.SweepOut(0f);

        Tale.Exec(() =>
        {
            StateManager.Instance.SwitchState(State.RoundTable);
        });

        Transition.SweepIn();

        Tale.Wait();

        Tale.Exec(() => ShowNextLaw());
    }

    private async void ShowNextLaw()
    {
        CurrentLaw = _lawManager.PickLaw();
        LibraryManager.Instance.Initialize();

        if (CurrentLaw == null)
        {
            EndGame();
            return;
        }

        RoundTableManager.Instance.ShowLawCard();
    }

    public void OnLawCardHidden()
    {
        RoundTableManager.Instance.ShowMoodBars();
    }

    public async void OnMoodBarsHidden()
    {
        Transition.SweepOut();

        Tale.Exec(() =>
        {
            StateManager.Instance.SwitchState(State.Beaureu);

            var npcInteractions = _npcManager.PickNPCs();
            BeaureauManager.Instance.SetQueue(npcInteractions);
            BeaureauManager.Instance.Initialize();
        });

        Transition.SweepIn();

        Tale.Exec(async () =>
        {
            await UniTask.Delay(1000);

            BeaureauManager.Instance.ShowNextNPC();
        });
    }

    public void OnBeaureauEnded()
    {
        Transition.SweepOut();

        Tale.Exec(() => StateManager.Instance.SwitchState(State.Library));

        LibraryManager.Instance.InitializeUI();

        Transition.SweepIn();
    }

    public void OnLibraryEnded()
    {
        Transition.SweepOut();

        Tale.Exec(() => StateManager.Instance.SwitchState(State.RoundTable));

        Transition.SweepIn();

        Tale.Exec(() => VoteLaw());
    }

    private void VoteLaw()
    {
        RoundTableManager.Instance.VoteLaw();
    }

    public void OnVoteEnded(bool lawApproved)
    {
        _roundIndex++;

        if (lawApproved)
        {
            foreach (var effect in CurrentLaw.Effects)
            {
                switch (effect.Type)
                {
                    case FactionType.Traditionalist:
                        _traditionalistPoints += effect.Value;
                        break;
                    case FactionType.Left:
                        _leftPoints += effect.Value;
                        break;
                    case FactionType.Right:
                        _rightPoints += effect.Value;
                        break;
                    case FactionType.Libertarian:
                        _libertarianPoints += effect.Value;
                        break;
                }
            }
        }

        if (_roundIndex < Config.Rounds)
        {
            ShowNextLaw();
        }
        else
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Transition.SweepOut();

        Tale.Exec(() => StateManager.Instance.SwitchState(State.GameEnd));

        Transition.SweepIn();

        Tale.Exec(() => GameEndManager.Instance.ShowGameEnd());
    }
}
