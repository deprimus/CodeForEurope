using UnityEngine;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{
    [Foldout("References")] public LawManager _lawManager;
    [Foldout("References")] public NPCManager _npcManager;
    

    public static GameManager Instance { get; private set; }

    public Law CurrentLaw { get; private set; }

    private int _roundIndex = 0;

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
        _npcManager.Initialize();

        StateManager.Instance.SwitchState(State.RoundTable);
        ShowNextLaw();
    }

    private async void ShowNextLaw()
    {
        CurrentLaw = _lawManager.PickLaw();

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
        StateManager.Instance.SwitchState(State.Beaureu);
        
        var npcInteractions = _npcManager.PickNPCs();
        foreach (var npcInteraction in npcInteractions)
        {
            BeaureauManager.Instance.AddNPC(npcInteraction);
        }
        BeaureauManager.Instance.Initialize();

        await UniTask.Delay(1000);

        BeaureauManager.Instance.ShowNextNPC();
    }

    public void OnBeaureauEnded()
    {
        StateManager.Instance.SwitchState(State.Library);
    }

    public void OnLibraryEnded()
    {
        StateManager.Instance.SwitchState(State.RoundTable);

        VoteLaw();
    }

    private void VoteLaw()
    {

    }

    public void OnVoteEnded()
    {
        _roundIndex++;

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
        // TODO: End game
    }
}
