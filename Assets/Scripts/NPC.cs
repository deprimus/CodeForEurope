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