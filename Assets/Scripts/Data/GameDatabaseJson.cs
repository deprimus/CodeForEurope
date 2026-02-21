using System;
using System.Collections.Generic;

[Serializable]
public class GameDatabaseRoot
{
    public List<NpcJson> npcs;
    public List<InteractionJson> interactions;
    public List<LawJson> laws;
}

[Serializable]
public class NpcJson
{
    public string id;
    public string name;
    public string prefabPath;
    public List<int> orientations;
}

[Serializable]
public class InteractionJson
{
    public string name;
    public string npcId;
    public List<string> dialogue;
    public List<InteractionEffectJson> effects;
}

[Serializable]
public class InteractionEffectJson
{
    public int type;
    public int value;
}

[Serializable]
public class LawJson
{
    public string name;
    public string description;
    public string iconPath;
    public List<LawEffectJson> effects;
    public List<string> interactionNames;
}

[Serializable]
public class LawEffectJson
{
    public int type;
    public int value;
}
