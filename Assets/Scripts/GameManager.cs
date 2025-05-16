using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int _roundIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        _roundIndex = 0;

        ShowNextLaw();
    }

    private void ShowNextLaw()
    {
        
    }

    public void OnLawCardHidden()
    {
        RoundTableManager.Instance.ShowMoodBars();
    }

    public void OnMoodBarsHidden()
    {
        
    }
}
