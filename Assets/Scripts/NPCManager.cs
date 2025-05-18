using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCManager", menuName = "Game/NPCManager")]
public class NPCManager : ScriptableObject
{
    public List<NPCInteraction> NPCInteractions = new List<NPCInteraction>();

    private List<NPCInteraction> _randomNPCInteractions = new List<NPCInteraction>();
    private List<NPCInteraction> _orientationNPCInteractions = new List<NPCInteraction>();

    public void Initialize()
    {
        _randomNPCInteractions = NPCInteractions.FindAll(interaction => interaction.NPC.Orientations.Count == 0);
        _orientationNPCInteractions = NPCInteractions.FindAll(interaction => interaction.NPC.Orientations.Count == 1);
    }

    public List<NPCInteraction> PickNPCs()
    {
        var npcs = new List<NPCInteraction>();
        npcs.Add(GameManager.Instance.CurrentLaw.NPCInteraction);

        for (int i = 0; i < 2; i++)
        {
            if (Random.Range(0, 100) < 50 && false)
            {
                if (_randomNPCInteractions.Count == 0)
                    continue;

                var randomNPCInteraction = _randomNPCInteractions[Random.Range(0, _randomNPCInteractions.Count)];
                npcs.Add(randomNPCInteraction);

                _randomNPCInteractions.Remove(randomNPCInteraction);
            }
            else
            {
                var orientationInteractions = new List<NPCInteraction>();
                foreach (var effect in GameManager.Instance.CurrentLaw.Effects)
                {
                    var orientationNPCInteractions = _orientationNPCInteractions.FindAll(interaction => interaction.NPC.Orientations.Contains(effect.Type));
                    orientationInteractions.AddRange(orientationNPCInteractions);
                }

                if (orientationInteractions.Count == 0)
                    continue;

                var orientationNPCInteraction = orientationInteractions[Random.Range(0, orientationInteractions.Count)];
                npcs.Add(orientationNPCInteraction);

                _orientationNPCInteractions.Remove(orientationNPCInteraction);
            }
        }
        
        return npcs;
    }
}
