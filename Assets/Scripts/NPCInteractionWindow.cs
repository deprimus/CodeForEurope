using UnityEditor;
using UnityEngine;

public class NPCInteractionWindow : EditorWindow
{
    private NPC selectedNPC;
    private string dialogueText;
    private GameData gameData;

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

        selectedNPC = (NPC)EditorGUILayout.ObjectField("NPC", selectedNPC, typeof(NPC), false);
        dialogueText = EditorGUILayout.TextField("Dialogue", dialogueText);

        if (GUILayout.Button("Save Interaction"))
        {
            if (selectedNPC != null && !string.IsNullOrEmpty(dialogueText))
            {
                SaveInteraction();
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
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

        if (gameData != null)
        {
            foreach (var interaction in gameData.NPCInteractions)
            {
                if (string.IsNullOrEmpty(searchQuery) || interaction.NPC.Name.ToLower().Contains(searchQuery.ToLower()))
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("NPC: " + interaction.NPC.Name);
                    string newDialogue = EditorGUILayout.TextField("Dialogue", interaction.Dialogue);
                    if (newDialogue != interaction.Dialogue)
                    {
                        interaction.Dialogue = newDialogue;
                        EditorUtility.SetDirty(gameData);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    if (GUILayout.Button("Delete Interaction"))
                    {
                        gameData.NPCInteractions.Remove(interaction);
                        EditorUtility.SetDirty(gameData);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        break;
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }
        else
        {
            LoadGameData();
        }

        GUILayout.EndScrollView();
    }

    private void LoadGameData()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameData");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            gameData = AssetDatabase.LoadAssetAtPath<GameData>(path);
        }
        else
        {
            Debug.LogError("GameData asset not found.");
        }
    }

    private void SaveInteraction()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameData");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            gameData = AssetDatabase.LoadAssetAtPath<GameData>(path);
        }
        else
        {
            Debug.LogError("GameData asset not found.");
            return;
        }

        NPCInteraction newInteraction = new NPCInteraction
        {
            NPC = selectedNPC,
            Dialogue = dialogueText
        };

        gameData.NPCInteractions.Add(newInteraction);
        EditorUtility.SetDirty(gameData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
