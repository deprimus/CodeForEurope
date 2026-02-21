using System;
using System.Collections.Generic;
using UnityEngine;

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

    AllLefts,
    AllRights,
    AllLibertarians,
    AllTraditionalists
}
