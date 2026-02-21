// -----------------------------------------------------------------------------
// LibraryManager.cs
//
// Manages the library scene, including tracking player actions (book/laptop usage),
// storing NPC interactions, and managing the UI for reviewing past choices and debunking misinformation.
//
// Main Functions:
// - Initialize(): Sets up the library state and clears previous data.
// - AddInteraction(): Stores an NPC interaction and the player's choice.
// - Debunk(): Handles the debunking process and updates law effects.
// - OnRevertApplied(): Resets the UI after a revert action.
//
// Fields:
// - _debunkButton, _continueButton: UI elements for player actions.
// - _lawManager: Reference to the LawManager.
// - _cardPrefab, _cardsParent: UI for displaying interaction cards.
// - _interactions: List of all player interactions.
// -----------------------------------------------------------------------------


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

    private bool _debunked;

    private List<(NPCInteraction, bool)> _interactions;
    private List<UIView_LibraryCard> _spawnedCards;

    private LibraryCamera _camera;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _camera = CameraManager.Instance.Camera.GetComponent<LibraryCamera>();
    }

    public void Initialize()
    {
        _debunked = false;
        _interactions = new List<(NPCInteraction, bool)>();

        if (_spawnedCards != null)
        {
            foreach (var card in _spawnedCards)
                Destroy(card.gameObject);
        }

        _spawnedCards = new List<UIView_LibraryCard>();
    }

    public void InitializeUI()
    {
        foreach (var interaction in _interactions)
        {
            var verdict = interaction.Item2 ? "<color=#009f00>accepted</color>" : "<color=#9f0000>rejected</color>";

            var card = Instantiate(_cardPrefab, _cardsParent);
            card.SetData(string.Format("{0}'s Proposal\n({1})", interaction.Item1.NPC.Name, verdict), interaction.Item1.Effects, interaction.Item2);
            _spawnedCards.Add(card);
        }
    }

    public void UseBook()
    {
    }

    public void UseLaptop()
    {
        var camera = CameraManager.Instance.Camera;
        _camera.MoveToLaptop();
    }

    public void AddInteraction(NPCInteraction interaction, bool option)
    {
        _interactions.Add((interaction, option));
    }

    public void Debunk()
    {
        _debunked = true;

        _lawManager.SetCurrentLawEffects(GameManager.Instance.CurrentLaw.Effects);

        foreach (var card in _spawnedCards)
            card.Debunk();
    }

    public void OnRevertApplied()
    {
        foreach (var card in _spawnedCards)
            card.gameObject.SetActive(false);
    }
}
