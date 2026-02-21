using System.Collections.Generic;

public class NPCManager
{
    public List<NPCInteraction> NPCInteractions { get; private set; }

    public NPCManager(List<NPCInteraction> interactions)
    {
        NPCInteractions = new List<NPCInteraction>(interactions);
    }

    public List<NPCInteraction> PickNPCs()
    {
        var npcs = new List<NPCInteraction>();
        npcs.AddRange(GameManager.Instance.CurrentLaw.NPCInteractions);
        return npcs;
    }
}
