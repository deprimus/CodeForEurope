#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class NPCInteractionWindow : EditorWindow
{
    private string interactionName;
    private int selectedNPCIndex;
    private List<string> dialogueText = new List<string>();
    private List<InteractionEffectJson> effects = new List<InteractionEffectJson>();

    private GameDatabaseRoot _database;
    private Vector2 scrollPosition;
    private string searchQuery = "";

    [MenuItem("Game/NPC Interaction Creator")]
    public static void ShowWindow()
    {
        GetWindow<NPCInteractionWindow>("NPC Interaction Creator");
    }

    private void OnEnable()
    {
        _database = GameDatabaseJsonIO.Load();
    }

    private void OnGUI()
    {
        if (_database == null)
        {
            EditorGUILayout.LabelField("No game_database.json found. Use Game > Export ScriptableObjects to JSON first.");
            if (GUILayout.Button("Reload"))
                _database = GameDatabaseJsonIO.Load();
            return;
        }

        GUILayout.Label("Create a new NPC Interaction", EditorStyles.boldLabel);

        interactionName = EditorGUILayout.TextField("Interaction Name", interactionName);

        var npcNames = _database.npcs.Select(n => n.id).ToArray();
        if (npcNames.Length > 0)
        {
            selectedNPCIndex = Mathf.Clamp(selectedNPCIndex, 0, npcNames.Length - 1);
            selectedNPCIndex = EditorGUILayout.Popup("NPC", selectedNPCIndex, npcNames);
        }
        else
        {
            EditorGUILayout.LabelField("No NPCs defined yet.");
        }

        GUILayout.Space(16);

        if (GUILayout.Button("Add Dialogue Line"))
            dialogueText.Add("");

        for (int i = 0; i < dialogueText.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            dialogueText[i] = EditorGUILayout.TextField($"Dialogue {i + 1}", dialogueText[i]);
            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                dialogueText.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(16);
        GUILayout.Label("Effects", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Effect"))
            effects.Add(new InteractionEffectJson());

        for (int i = 0; i < effects.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            effects[i].type = (int)(InteractionEffectType)EditorGUILayout.EnumPopup("Effect Type", (InteractionEffectType)effects[i].type);
            effects[i].value = EditorGUILayout.IntField("Value", effects[i].value);
            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                effects.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(16);

        if (GUILayout.Button("Save Interaction"))
        {
            if (npcNames.Length > 0 && dialogueText.Count > 0 && !string.IsNullOrEmpty(interactionName))
            {
                _database.interactions.Add(new InteractionJson
                {
                    name = interactionName,
                    npcId = npcNames[selectedNPCIndex],
                    dialogue = new List<string>(dialogueText),
                    effects = effects.Select(e => new InteractionEffectJson { type = e.type, value = e.value }).ToList()
                });

                GameDatabaseJsonIO.Save(_database);

                interactionName = "";
                dialogueText = new List<string>();
                effects = new List<InteractionEffectJson>();
            }
            else
            {
                Debug.LogError("Name, NPC, and at least one Dialogue line are required.");
            }
        }

        GUILayout.Space(20);
        GUILayout.Label("Search NPC Interactions", EditorStyles.boldLabel);
        searchQuery = EditorGUILayout.TextField("Search", searchQuery);

        GUILayout.Space(10);
        GUILayout.Label("NPC Interactions", EditorStyles.boldLabel);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < _database.interactions.Count; i++)
        {
            var interaction = _database.interactions[i];
            if (!string.IsNullOrEmpty(searchQuery) && !interaction.npcId.ToLower().Contains(searchQuery.ToLower())
                && !interaction.name.ToLower().Contains(searchQuery.ToLower()))
                continue;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("NPC: " + interaction.npcId);

            string newName = EditorGUILayout.TextField("Interaction Name", interaction.name);
            if (newName != interaction.name)
            {
                var oldName = interaction.name;
                interaction.name = newName;

                foreach (var law in _database.laws)
                {
                    for (int li = 0; li < law.interactionNames.Count; li++)
                    {
                        if (law.interactionNames[li] == oldName)
                            law.interactionNames[li] = newName;
                    }
                }

                GameDatabaseJsonIO.Save(_database);
            }

            GUILayout.Space(8);

            if (GUILayout.Button("Add Dialogue Line"))
            {
                interaction.dialogue.Add("");
                GameDatabaseJsonIO.Save(_database);
            }

            for (int j = 0; j < interaction.dialogue.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                string newDialogue = EditorGUILayout.TextField($"Dialogue {j + 1}", interaction.dialogue[j]);
                if (newDialogue != interaction.dialogue[j])
                {
                    interaction.dialogue[j] = newDialogue;
                    GameDatabaseJsonIO.Save(_database);
                }
                if (GUILayout.Button("Delete", GUILayout.Width(100)))
                {
                    interaction.dialogue.RemoveAt(j);
                    GameDatabaseJsonIO.Save(_database);
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(8);
            GUILayout.Label("Effects", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Effect"))
            {
                interaction.effects.Add(new InteractionEffectJson());
                GameDatabaseJsonIO.Save(_database);
            }

            for (int k = 0; k < interaction.effects.Count; k++)
            {
                EditorGUILayout.BeginHorizontal();
                var newType = (int)(InteractionEffectType)EditorGUILayout.EnumPopup("Effect Type", (InteractionEffectType)interaction.effects[k].type);
                if (newType != interaction.effects[k].type)
                {
                    interaction.effects[k].type = newType;
                    GameDatabaseJsonIO.Save(_database);
                }
                int newValue = EditorGUILayout.IntField("Value", interaction.effects[k].value);
                if (newValue != interaction.effects[k].value)
                {
                    interaction.effects[k].value = newValue;
                    GameDatabaseJsonIO.Save(_database);
                }
                if (GUILayout.Button("Delete", GUILayout.Width(100)))
                {
                    interaction.effects.RemoveAt(k);
                    GameDatabaseJsonIO.Save(_database);
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(8);

            if (GUILayout.Button("Delete Interaction"))
            {
                foreach (var law in _database.laws)
                    law.interactionNames.Remove(interaction.name);

                _database.interactions.RemoveAt(i);
                GameDatabaseJsonIO.Save(_database);
            }

            EditorGUILayout.EndVertical();
        }

        GUILayout.EndScrollView();
    }
}
#endif
