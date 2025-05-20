using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    [Foldout("References")] public GameObject _debunkButton;
    [Foldout("References")] public GameObject _continueButton;
    [Foldout("References")] public GameObject _revealButton;
    [Foldout("References")] public LawManager _lawManager;
    [Foldout("References")] public Transform _choiceCardsContainer;
    [Foldout("References")] public GameObject _choiceCardPrefab;

    private List<BureauChoice> _bureauChoices = new List<BureauChoice>();
    private List<UIView_ChoiceCard> _choiceCards = new List<UIView_ChoiceCard>();
    private bool _canRevertChoice = true;

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

    private void UpdateUI()
    {
        _debunkButton.SetActive(_usedBook && _usedLaptop && !_debunked);
        _continueButton.SetActive(_debunked);
    }

    public void AddBureauChoice(string description, List<LawEffect> effects)
    {
        _bureauChoices.Add(new BureauChoice
        {
            Description = description,
            Effects = effects,
            IsRevealed = false
        });
    }

    public void Debunk()
    {
        _debunked = true;
        _revealButton.SetActive(true);
        SpawnChoiceCards();
        UpdateUI();
        _lawManager.SetCurrentLawEffects(GameManager.Instance.CurrentLaw.Effects);
    }

    private void SpawnChoiceCards()
    {
        foreach (var choice in _bureauChoices)
        {
            var cardGO = Instantiate(_choiceCardPrefab, _choiceCardsContainer);
            var card = cardGO.GetComponent<UIView_ChoiceCard>();
            card.Initialize(choice);
            _choiceCards.Add(card);
        }
    }

    public void RevealNextCard()
    {
        var unrevealed = _choiceCards.FirstOrDefault(c => !c.Choice.IsRevealed);
        if (unrevealed != null)
        {
            unrevealed.Reveal();
        }

        if (_choiceCards.All(c => c.Choice.IsRevealed))
        {
            _revealButton.SetActive(false);
        }
    }

    public void RevertChoice(UIView_ChoiceCard card)
    {
        if (!_canRevertChoice) return;

        _canRevertChoice = false;
        // Here you would implement the logic to revert the effects of this choice
        _continueButton.SetActive(true);
    }
}
