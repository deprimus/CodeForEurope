using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NPCInteractionWindow : EditorWindow
{
    private string interactionName;
    private NPC selectedNPC;
    private List<string> dialogueText = new List<string>();
    private List<InteractionEffect> effects = new List<InteractionEffect>();
    private NPCManager npcManager;

    [MenuItem("Game/NPC Interaction Creator")]
    public static void ShowWindow()
    {
        GetWindow<NPCInteractionWindow>("NPC Interaction Creator");
    }

    private Vector2 scrollPosition;
    private string searchQuery = "";

    private void OnGUI()
    {
        GUILayout.Label("Create a new NPC Interaction", EditorStyles.boldLabel);

        interactionName = EditorGUILayout.TextField("Interaction Name", interactionName);
        selectedNPC = (NPC)EditorGUILayout.ObjectField("NPC", selectedNPC, typeof(NPC), false);

        GUILayout.Space(16);

        if (GUILayout.Button("Add Dialogue Line"))
        {
            dialogueText.Add("");
        }

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
        {
            effects.Add(new InteractionEffect());
        }

        for (int i = 0; i < effects.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            effects[i].Type = (InteractionEffectType)EditorGUILayout.EnumPopup("Effect Type", effects[i].Type);
            effects[i].Value = EditorGUILayout.IntField("Value", effects[i].Value);
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
            if (selectedNPC != null && dialogueText.Count > 0)
            {
                SaveInteraction();

                selectedNPC = null;
                dialogueText = new List<string>();
                effects = new List<InteractionEffect>();
                interactionName = "";
            }
            else
            {
                Debug.LogError("NPC and Dialogue must be set.");
            }
        }

        GUILayout.Space(20);
        GUILayout.Label("Search NPC Interactions", EditorStyles.boldLabel);
        searchQuery = EditorGUILayout.TextField("Search", searchQuery);

        GUILayout.Space(10);
        GUILayout.Label("NPC Interactions", EditorStyles.boldLabel);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        if (npcManager != null)
        {
            for (int i = 0; i < npcManager.NPCInteractions.Count; i++)
            {
                var interaction = npcManager.NPCInteractions[i];
                if (string.IsNullOrEmpty(searchQuery) || interaction.NPC.Name.ToLower().Contains(searchQuery.ToLower()))
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("NPC: " + interaction.NPC.Name);
                    string newName = EditorGUILayout.TextField("Interaction Name", interaction.Name);
                    if (newName != interaction.Name)
                    {
                        interaction.Name = newName;
                        EditorUtility.SetDirty(npcManager);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    GUILayout.Space(16);

                    if (GUILayout.Button("Add Dialogue Line"))
                    {
                        interaction.Dialogue.Add("");
                        EditorUtility.SetDirty(npcManager);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    GUILayout.Space(16);

                    for (int j = 0; j < interaction.Dialogue.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        string newDialogue = EditorGUILayout.TextField($"Dialogue {j + 1}", interaction.Dialogue[j]);
                        if (newDialogue != interaction.Dialogue[j])
                        {
                            interaction.Dialogue[j] = newDialogue;
                            EditorUtility.SetDirty(npcManager);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        if (GUILayout.Button("Delete", GUILayout.Width(100)))
                        {
                            interaction.Dialogue.RemoveAt(j);
                            EditorUtility.SetDirty(npcManager);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    GUILayout.Space(16);

                    if (GUILayout.Button("Add Effect"))
                    {
                        interaction.Effects.Add(new InteractionEffect());
                        EditorUtility.SetDirty(npcManager);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    GUILayout.Space(16);

                    for (int k = 0; k < interaction.Effects.Count; k++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        InteractionEffectType newType = (InteractionEffectType)EditorGUILayout.EnumPopup("Effect Type", interaction.Effects[k].Type);
                        if (newType != interaction.Effects[k].Type)
                        {
                            interaction.Effects[k].Type = newType;
                            EditorUtility.SetDirty(npcManager);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        int newValue = EditorGUILayout.IntField("Effect Value", interaction.Effects[k].Value);
                        if (newValue != interaction.Effects[k].Value)
                        {
                            interaction.Effects[k].Value = newValue;
                            EditorUtility.SetDirty(npcManager);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        if (GUILayout.Button("Delete Effect", GUILayout.Width(100)))
                        {
                            interaction.Effects.RemoveAt(k);
                            EditorUtility.SetDirty(npcManager);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    GUILayout.Space(16);

                    if (GUILayout.Button("Delete Interaction"))
                    {
                        npcManager.NPCInteractions.Remove(interaction);
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(interaction));
                        EditorUtility.SetDirty(npcManager);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }
        else
        {
            LoadNPCManager();
        }

        GUILayout.EndScrollView();
    }

    private void LoadNPCManager()
    {
        string[] guids = AssetDatabase.FindAssets("t:NPCManager");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            npcManager = AssetDatabase.LoadAssetAtPath<NPCManager>(path);
        }
        else
        {
            Debug.LogError("NPCManager asset not found.");
        }
    }

    private void SaveInteraction()
    {
        string[] guids = AssetDatabase.FindAssets("t:NPCManager");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            npcManager = AssetDatabase.LoadAssetAtPath<NPCManager>(path);
        }
        else
        {
            Debug.LogError("NPCManager asset not found.");
            return;
        }

        NPCInteraction newInteraction = ScriptableObject.CreateInstance<NPCInteraction>();
        newInteraction.Name = interactionName;
        newInteraction.NPC = selectedNPC;
        newInteraction.Dialogue = new List<string>(dialogueText);
        newInteraction.Effects = new List<InteractionEffect>(effects);

        AssetDatabase.CreateAsset(newInteraction, $"Assets/ScriptableObjects/NPCInteractions/{interactionName}_Interaction.asset");
        AssetDatabase.SaveAssets();

        npcManager.NPCInteractions.Add(newInteraction);
        EditorUtility.SetDirty(npcManager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
