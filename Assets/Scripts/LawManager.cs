using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "LawManager", menuName = "Game/LawManager")]
public class LawManager : ScriptableObject
{
    private List<Law> _laws = new List<Law> {
        new Law {
            Name = "Law 1",
            Description = "Description 1",
        },
        new Law {
            Name = "Law 2",
            Description = "Description 2",
        },
        new Law {
            Name = "Law 3",
            Description = "Description 3",
        }
    };

    public Law PickLaw()
    {
        return _laws[Random.Range(0, _laws.Count)];
    }
}