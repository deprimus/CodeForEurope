// -----------------------------------------------------------------------------
// NPCInteraction.cs
//
// ScriptableObject representing a single NPC interaction (dialogue, choices, and effects).
// Used to define how an NPC interacts with the player, including dialogue lines and the effects of choices.
//
// Fields:
// - Name: The name of the interaction.
// - NPC: The NPC involved in this interaction.
// - Dialogue: List of dialogue lines for the interaction.
// - Effects: List of effects (faction influence) resulting from the interaction.
//
// Classes:
// - InteractionEffect: Represents a single effect (type and value) on a faction.
// - InteractionEffectType: Enum of possible factions/effects.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCInteraction", menuName = "Game/NPCInteraction")]
public class NPCInteraction : ScriptableObject
{
    public string Name;
    public NPC NPC;
    public List<string> Dialogue;
    public List<InteractionEffect> Effects;
}

[Serializable]
public class InteractionEffect
{
    public InteractionEffectType Type;
    public int Value;
}

public enum InteractionEffectType
{
    GreensParty,
    TraditionalistParty,
    ProgressistParty,
    LiberalParty,

    Lefts,
    Rights,
    Libertarians,
    Traditionalists
}