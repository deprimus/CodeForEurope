using System.Collections.Generic;
using UnityEngine;

public class NPC : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public List<FactionType> Orientations;
}
