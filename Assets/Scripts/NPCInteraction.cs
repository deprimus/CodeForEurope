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