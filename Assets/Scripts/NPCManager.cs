using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCManager", menuName = "Game/NPCManager")]
public class NPCManager : ScriptableObject
{
    [Foldout("References")] public GameData _gameData;

    public NPCInteraction GetRandomNPCInteraction()
    {
        return _gameData.NPCInteractions[Random.Range(0, _gameData.NPCInteractions.Count)];
    }

    public List<NPCInteraction> GetAllNPCInteractions()
    {
        return _gameData.NPCInteractions;
    }
}
