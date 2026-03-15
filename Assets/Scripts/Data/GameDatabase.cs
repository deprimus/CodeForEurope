using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameDatabase : MonoBehaviour
{
    public static GameDatabase Instance { get; private set; }

    private Dictionary<string, NPC> _npcLookup;
    private Dictionary<string, NPCInteraction> _interactionLookup;
    private List<Law> _laws;
    private Dictionary<string, List<PostJson>> _postsByLawName;

    private LawManager _lawManager;
    private NPCManager _npcManager;

    public LawManager LawManager => _lawManager;
    public NPCManager NPCManager => _npcManager;

    public IReadOnlyDictionary<string, NPC> NPCs => _npcLookup;
    public IReadOnlyDictionary<string, NPCInteraction> Interactions => _interactionLookup;
    public IReadOnlyList<Law> Laws => _laws;

    public List<PostJson> GetPostsForLaw(string lawName)
    {
        if (_postsByLawName != null && _postsByLawName.TryGetValue(lawName, out var posts))
            return posts;
        return new List<PostJson>();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Load();
    }

    private void Load()
    {
        var json = Resources.Load<TextAsset>("GameData/game_database");
        if (json == null)
        {
            Debug.LogError("GameDatabase: game_database.json not found in Resources/GameData/");
            return;
        }

        var data = JsonUtility.FromJson<GameDatabaseRoot>(json.text);

        BuildNPCs(data.npcs);
        BuildInteractions(data.interactions);
        BuildLaws(data.laws);
        BuildPosts(data.posts);

        _lawManager = new LawManager(_laws);
        _npcManager = new NPCManager(_interactionLookup.Values.ToList());
    }

    private void BuildNPCs(List<NpcJson> npcData)
    {
        _npcLookup = new Dictionary<string, NPC>();

        foreach (var entry in npcData)
        {
            GameObject prefab = null;
            if (!string.IsNullOrEmpty(entry.prefabPath))
            {
                prefab = Resources.Load<GameObject>(entry.prefabPath);
                if (prefab == null)
                    Debug.LogWarning($"GameDatabase: Prefab not found at '{entry.prefabPath}' for NPC '{entry.id}'");
            }

            var npc = ScriptableObject.CreateInstance<NPC>();
            npc.Name = entry.name;
            npc.Prefab = prefab;
            npc.Orientations = entry.orientations.Select(o => (FactionType)o).ToList();
            npc.hideFlags = HideFlags.HideAndDontSave;

            _npcLookup[entry.id] = npc;
        }
    }

    private void BuildInteractions(List<InteractionJson> interactionData)
    {
        _interactionLookup = new Dictionary<string, NPCInteraction>();

        foreach (var entry in interactionData)
        {
            if (!_npcLookup.TryGetValue(entry.npcId, out var npc))
            {
                Debug.LogWarning($"GameDatabase: NPC '{entry.npcId}' not found for interaction '{entry.name}'");
                continue;
            }

            var interaction = ScriptableObject.CreateInstance<NPCInteraction>();
            interaction.Name = entry.name;
            interaction.NPC = npc;
            interaction.Dialogue = new List<string>(entry.dialogue);
            interaction.Effects = entry.effects.Select(e => new InteractionEffect
            {
                Type = (InteractionEffectType)e.type,
                Value = e.value
            }).ToList();
            interaction.hideFlags = HideFlags.HideAndDontSave;

            _interactionLookup[entry.name] = interaction;
        }
    }

    private void BuildLaws(List<LawJson> lawData)
    {
        _laws = new List<Law>();

        foreach (var entry in lawData)
        {
            Sprite icon = null;
            if (!string.IsNullOrEmpty(entry.iconPath))
                icon = Resources.Load<Sprite>(entry.iconPath);

            var npcInteractions = new List<NPCInteraction>();
            foreach (var interactionName in entry.interactionNames)
            {
                if (_interactionLookup.TryGetValue(interactionName, out var interaction))
                    npcInteractions.Add(interaction);
                else
                    Debug.LogWarning($"GameDatabase: Interaction '{interactionName}' not found for law '{entry.name}'");
            }

            _laws.Add(new Law
            {
                Name = entry.name,
                Description = entry.description,
                Icon = icon,
                Effects = entry.effects.Select(e => new LawEffect
                {
                    Type = (FactionType)e.type,
                    Value = e.value
                }).ToList(),
                WelfareEffects = entry.welfareEffects != null
                    ? entry.welfareEffects.Select(e => new WelfareEffect
                    {
                        Indicator = (WelfareIndicator)e.indicator,
                        Value = e.value
                    }).ToList()
                    : new List<WelfareEffect>(),
                NPCInteractions = npcInteractions
            });
        }
    }

    private void BuildPosts(List<LawPostsJson> postsData)
    {
        _postsByLawName = new Dictionary<string, List<PostJson>>();
        if (postsData == null) return;

        foreach (var entry in postsData)
            _postsByLawName[entry.lawName] = entry.posts;
    }

    private void OnDestroy()
    {
        if (_npcLookup != null)
        {
            foreach (var npc in _npcLookup.Values)
                if (npc != null) DestroyImmediate(npc);
        }

        if (_interactionLookup != null)
        {
            foreach (var interaction in _interactionLookup.Values)
                if (interaction != null) DestroyImmediate(interaction);
        }
    }
}
