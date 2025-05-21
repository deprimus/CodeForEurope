// -----------------------------------------------------------------------------
// NPCManager.cs
//
// ScriptableObject that manages all NPC interactions in the game.
// Stores a list of all NPCInteraction assets and provides methods to retrieve them for gameplay.
//
// Main Functions:
// - PickNPCs(): Returns the list of NPC interactions relevant to the current law.
//
// Fields:
// - NPCInteractions: List of all NPCInteraction assets in the project.
// -----------------------------------------------------------------------------

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
