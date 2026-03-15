using UnityEngine;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
public class GameManager : MonoBehaviour
{
    public int TraditionalistPoints => _traditionalistPoints;
    public int LeftPoints => _leftPoints;
    public int RightPoints => _rightPoints;
    public int LibertarianPoints => _libertarianPoints;

    public static GameManager Instance { get; private set; }

    public Law CurrentLaw { get; private set; }

    public int UserLawInfluence => _userLawInfluence;
    public WelfareManager Welfare { get; private set; }

    private int _roundIndex = 0;

    [Foldout("Debug"), SerializeField, ReadOnly]
    private int _traditionalistPoints;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private int _leftPoints;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private int _rightPoints;
    [Foldout("Debug"), SerializeField, ReadOnly]
    private int _libertarianPoints;

    public static int BaseUserWinPercentage = 60;
    
    private int _userLawInfluence = 0;
    private LawManager _lawManager;
    private NPCManager _npcManager;

    private async void Awake()
    {
        Instance = this;

        await UniTask.Delay(500);

        StartGame();
    }

    public void StartGame()
    {
        _roundIndex = 0;
        Welfare = new WelfareManager();

        if (GameDatabase.Instance == null)
        {
            Debug.LogError("GameDatabase not found in scene. Add a GameObject with the GameDatabase component.");
            return;
        }

        _lawManager = GameDatabase.Instance.LawManager;
        _npcManager = GameDatabase.Instance.NPCManager;

        _lawManager.Initialize();

        Tale.Exec(() =>
        {
            StateManager.Instance.SwitchState(State.RoundTable);
        });

        Tale.Exec(() => RoundTableManager.Instance.SetCameraAnimation("before_transition"));

        Transition.SweepIn();

        Tale.Wait(0.55f);

        Tale.Exec(() => RoundTableManager.Instance.SetCameraAnimation("transition"));

        Tale.Wait(3);

        Tale.Exec(() => ShowNextLaw());
    }

    private async void ShowNextLaw()
    {
        _userLawInfluence = 0;

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

        Tale.Wait();

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

        Tale.Wait();

        Tale.Exec(() => StateManager.Instance.SwitchState(State.Library));

        LibraryManager.Instance.InitializeUI();

        Transition.SweepIn();

        // TODO: Re-enable auto-advance after testing EuroChat/Laptop UI
        Tale.Wait(1f);
        Tale.Exec(() => OnLibraryEnded());
    }

    public void OnLibraryEnded()
    {
        Transition.SweepOut();

        Tale.Wait();

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

            Welfare.ApplyEffects(CurrentLaw.WelfareEffects);
        }
        else
        {
            Welfare.ApplyEffects(CurrentLaw.WelfareEffects, -1f);
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

        Tale.Exec(() =>
        {
            StateManager.Instance.SwitchState(State.GameEnd);
            GameEndManager.Instance.ShowGameEnd();
        });
    }
}
