using UnityEngine;

public class RoundTableManager : MonoBehaviour
{
    public static RoundTableManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowMoodBars()
    {

    }
}
