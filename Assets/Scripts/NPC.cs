using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "Game/NPC")]
public class NPC : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
}

[System.Serializable]
public class NPCInteraction
{
    public NPC NPC;
    public string Dialogue;
}