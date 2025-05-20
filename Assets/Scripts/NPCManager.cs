using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCManager", menuName = "Game/NPCManager")]
public class NPCManager : ScriptableObject
{
    public List<NPCInteraction> NPCInteractions = new List<NPCInteraction>();

    public List<NPCInteraction> PickNPCs()
    {
        var npcs = new List<NPCInteraction>();
        npcs.AddRange(GameManager.Instance.CurrentLaw.NPCInteractions);
        return npcs;
    }
}
