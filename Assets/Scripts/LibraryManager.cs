using System.Collections.Generic;
using NaughtyAttributes;
using TaleUtil;
using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    [Foldout("References")] public GameObject _debunkButton;
    [Foldout("References")] public GameObject _continueButton;
    [Foldout("References")] public UIView_LibraryCard _cardPrefab;
    [Foldout("References")] public Transform _cardsParent;

    [Foldout("References")] public GameObject _libraryBookUI;

    public static LibraryManager Instance;

    private bool _debunked;

    private List<(NPCInteraction, bool)> _interactions;
    private List<UIView_LibraryCard> _spawnedCards;

    private LibraryCamera _camera;
    private LawManager _lawManager;

    private BookBehavior _bookBehavior;

    private bool _usedBook = false;
    private bool _usedLaptop = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _usedBook = false;
        _usedLaptop = false;
        _camera = CameraManager.Instance.Camera.GetComponent<LibraryCamera>();
        _bookBehavior = _libraryBookUI.GetComponent<BookBehavior>();
        if (_bookBehavior == null)
        {
            Log.Error("LibraryManager", "BookBehavior component not found on LibraryBookUI");
            return;
        }
        _bookBehavior.OnBookClosed += OnBookClosed;
    }

    public void Initialize()
    {
        _lawManager = GameDatabase.Instance.LawManager;

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
        UIBookPage.RulePageData pageData = new UIBookPage.RulePageData();
        pageData.title = GameManager.Instance.CurrentLaw.Name;
        pageData.description = GameManager.Instance.CurrentLaw.Description;
        pageData.longDescription = "";
        pageData.effects = new List<string>();
        foreach (var effect in GameManager.Instance.CurrentLaw.WelfareEffects)
        {
            pageData.effects.Add($"{effect.Indicator}: {effect.Value}");
        }
        pageData.effectsAreShown = false;
        _bookBehavior.AddPage(pageData);
        _libraryBookUI.SetActive(true);
    }

    public void OnBookClosed()
    {
        Log.Info("LibraryManager", "OnBookClose");
        Debunk();
        _usedBook = true;
        CheckFinished();
    }

    public void UseLaptop()
    {
        if (_camera == null)
            _camera = CameraManager.Instance.Camera.GetComponent<LibraryCamera>();

        _camera.MoveToLaptop();
    }

    public void OnLaptopClosed()
    {
        RestoreCamera();
        _usedLaptop = true;
        CheckFinished();
    }

    private void RestoreCamera()
    {
        if (_camera == null)
            _camera = CameraManager.Instance.Camera.GetComponent<LibraryCamera>();

        _camera.RestoreCamera();
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

    private void CheckFinished()
    {
        if (_usedBook && _usedLaptop)
        {
            _usedBook = false;
            _usedLaptop = false;
            Tale.Wait(1f);
            Tale.Exec(() => GameManager.Instance.OnLibraryEnded());
        }
    }
}
