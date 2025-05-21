using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    [Foldout("References")] public GameObject _debunkButton;
    [Foldout("References")] public GameObject _continueButton;
    [Foldout("References")] public LawManager _lawManager;
    [Foldout("References")] public UIView_LibraryCard _cardPrefab;
    [Foldout("References")] public Transform _cardsParent;

    public static LibraryManager Instance;

    private bool _usedBook;
    private bool _usedLaptop;
    private bool _debunked;

    private List<(NPCInteraction, bool)> _interactions;
    private List<UIView_LibraryCard> _spawnedCards;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        _usedBook = false;
        _usedLaptop = false;
        _debunked = false;
        _interactions = new List<(NPCInteraction, bool)>();

        if (_spawnedCards != null)
        {
            foreach (var card in _spawnedCards)
                Destroy(card.gameObject);
        }

        _spawnedCards = new List<UIView_LibraryCard>();

        UpdateUI();
    }

    public void InitializeUI()
    {
        Debug.Log(_interactions.Count);
        foreach (var interaction in _interactions)
        {
            var card = Instantiate(_cardPrefab, _cardsParent);
            card.SetData(interaction.Item1.Name, interaction.Item1.Effects, interaction.Item2);
            _spawnedCards.Add(card);
        }
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

    public void AddInteraction(NPCInteraction interaction, bool option)
    {
        _interactions.Add((interaction, option));
    }

    public void Debunk()
    {
        _debunked = true;

        UpdateUI();

        _lawManager.SetCurrentLawEffects(GameManager.Instance.CurrentLaw.Effects);

        foreach (var card in _spawnedCards)
            card.Debunk();
    }

    public void OnRevertApplied()
    {
        foreach (var card in _spawnedCards)
            card.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        _debunkButton.SetActive(_usedBook && _usedLaptop && !_debunked);
        _continueButton.SetActive(_debunked);
    }
}
