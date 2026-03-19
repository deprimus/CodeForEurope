using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TaleUtil;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LibraryManager : MonoBehaviour
{
    [Foldout("References")] public GameObject _debunkButton;
    [Foldout("References")] public GameObject _continueButton;
    [Foldout("References")] public UIView_LibraryCard _cardPrefab;
    [Foldout("References")] public Transform _cardsParent;

    [Foldout("References")] public GameObject _libraryBookUI;
    [Foldout("References")] public CanvasGroup _libraryBookCanvasGroup;

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
        var law = GameManager.Instance.CurrentLaw;
        if (law == null)
        {
            // For debugging
            law = GameDatabase.Instance.Laws[0];
        }
        pageData.title = law.Name;
        pageData.description = law.Description;
        pageData.longDescription = "";
        pageData.effects = new List<string>();
        foreach (var effect in law.WelfareEffects)
        {
            pageData.effects.Add($"{effect.Indicator}: {effect.Value}");
        }
        pageData.effectsAreShown = false;
        _bookBehavior.AddPage(pageData);
        _libraryBookUI.SetActive(true);
        _libraryBookCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutCubic);
        _libraryBookCanvasGroup.blocksRaycasts = true;
        _libraryBookCanvasGroup.interactable = true;
    }

    public void OnBookClosed()
    {
        _libraryBookCanvasGroup.DOFade(0, 0.5f).SetEase(Ease.InCubic)
            .OnComplete(() => {
                _libraryBookUI.SetActive(false);
                Debunk();
            });
        _libraryBookCanvasGroup.blocksRaycasts = false;
        _libraryBookCanvasGroup.interactable = false;
        _usedBook = true;
        CheckButtonVisibility();
    }

    public void UseLaptop()
    {
        if (_camera == null)
            _camera = CameraManager.Instance.Camera.GetComponent<LibraryCamera>();

        _camera.MoveToLaptop();
    }

    public void OnLaptopClosed()
    {
        Debug.Log("Laptop closed");
        RestoreCamera();
        _usedLaptop = true;
        CheckButtonVisibility();
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

    private void CheckButtonVisibility()
    {
        Debug.Log("Checking button visibility");
        Debug.Log("Used book: " + _usedBook);
        Debug.Log("Used laptop: " + _usedLaptop);
        if (_usedBook && _usedLaptop)
        {
            LibraryNextButton.Instance.Show();
        }
    }
}
