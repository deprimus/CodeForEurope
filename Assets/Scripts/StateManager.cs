// -----------------------------------------------------------------------------
// StateManager.cs
//
// Controls scene transitions and the current state of the game (e.g., RoundTable, Bureau, Library, GameEnd).
// Provides methods to switch between scenes and manage the active state.
//
// Main Functions:
// - SwitchState(State newState): Changes the active scene/state.
//
// Fields:
// - _scenes: Array of GameObjects representing each scene.
// - _currentState: The current game state.
// -----------------------------------------------------------------------------

using NaughtyAttributes;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [Foldout("Components"), SerializeField] private GameObject[] _scenes;

    public static StateManager Instance { get; private set; }

    private State _currentState;

    private void Awake()
    {
        Instance = this;
    }

    public void SwitchState(State newState)
    {
        _scenes[(int)_currentState].SetActive(false);
        _scenes[(int)newState].SetActive(true);

        _currentState = newState;
    }
}

public enum State
{
    RoundTable,
    Beaureu,
    Library,
    GameEnd
}