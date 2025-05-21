// -----------------------------------------------------------------------------
// NPC.cs
//
// ScriptableObject representing a Non-Player Character (NPC) in the game.
// Stores the NPC's name, associated prefab, and political orientations (factions).
// Used by the NPC Creator tool and referenced in interactions and gameplay.
//
// Fields:
// - Name: The display name of the NPC.
// - Prefab: The 3D model prefab for the NPC.
// - Orientations: List of political/faction alignments for the NPC.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "Game/NPC")]
public class NPC : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public List<FactionType> Orientations;
}