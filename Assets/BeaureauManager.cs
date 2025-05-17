using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class BeaureauManager : MonoBehaviour
{
    [Foldout("References")] public Transform _NPCSpawnPoint;
    [Foldout("References")] public Transform _NPCArrivalPoint;

    public static BeaureauManager Instance;

    private List<NPCInteraction> _npcQueue = new List<NPCInteraction>();

    private List<NPCView> _npcs = new List<NPCView>();

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

        var npc = _npcs[0];
        _npcs.RemoveAt(0);

        npc.BeginInteraction();
    }

    public void AddNPC(NPCInteraction npc)
    {
        _npcQueue.Add(npc);
    }
}
