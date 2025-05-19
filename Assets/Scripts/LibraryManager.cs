using NaughtyAttributes;
using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    [Foldout("References")] public GameObject _debunkButton;
    [Foldout("References")] public GameObject _continueButton;
    [Foldout("References")] public LawManager _lawManager;

    public static LibraryManager Instance;

    private bool _usedBook;
    private bool _usedLaptop;
    private bool _debunked;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        _usedBook = false;
        _usedLaptop = false;
        _debunked = false;
        
        UpdateUI();
    }

    public void UseBook()
    {
        _usedBook = true;

        UpdateUI();
    }

    public void UseLaptop()
    {
        _usedLaptop = true;

        UpdateUI();
    }

    public void Debunk()
    {
        _debunked = true;

        UpdateUI();

        _lawManager.SetCurrentLawEffects(GameManager.Instance.CurrentLaw.Effects);
    }

    private void UpdateUI()
    {
        _debunkButton.SetActive(_usedBook && _usedLaptop && !_debunked);
        _continueButton.SetActive(_debunked);
    }
}
