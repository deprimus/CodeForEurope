using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void SwitchState(State newState)
    {
        //_currentState = newState;
    }
}

public enum State
{
    RoundTable,
    Beauregard,
    Library,
}