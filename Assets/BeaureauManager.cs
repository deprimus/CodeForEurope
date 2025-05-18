using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class BeaureauManager : MonoBehaviour
{
    [Foldout("References")] public Transform _NPCSpawnPoint;
    [Foldout("References")] public Transform _NPCArrivalPoint;
    [Foldout("References")] public UIView_BeaureauPrompt _beaureauPrompt;
    [Foldout("References")] public RoundTableManager _roundTableManager;
    public static BeaureauManager Instance;

    private List<NPCInteraction> _npcQueue = new List<NPCInteraction>();

    private List<NPCView> _npcs = new List<NPCView>();

    private NPCView _currentNPC;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        foreach (var npc in _npcQueue)
        {
            var npcView = Instantiate(npc.NPC.Prefab, _NPCSpawnPoint).GetComponent<NPCView>();
            _npcs.Add(npcView);

            npcView.Initialize(npc, _NPCSpawnPoint.position, _NPCArrivalPoint.position);
        }
    }

    public void ShowNextNPC()
    {
        if (_npcs.Count == 0)
        {
            GameManager.Instance.OnBeaureauEnded();
            return;
        }

        _currentNPC = _npcs[0];
        _npcs.RemoveAt(0);

        _currentNPC.BeginInteraction();
    }

    public void OnInteractionEnded()
    {
        _beaureauPrompt.Show(OnOptionPicked);
    }

    private async void OnOptionPicked(bool option)
    {
        var effects = _currentNPC.Interaction.Effects;

        foreach (var effect in effects)
        {
            var value = option ? effect.Value : -effect.Value;
            _roundTableManager.Influence(effect.Type, value);
        }

        await _currentNPC.OnChoicePicked();
        
        ShowNextNPC();
    }

    public void AddNPC(NPCInteraction npc)
    {
        _npcQueue.Add(npc);
    }
}
