using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
public class NPCWindow : EditorWindow
{
    private string npcName;
    private GameObject model3D;
    private List<FactionType> orientations = new List<FactionType>();
    private float moveSpeed = 5f;

    private Vector2 scrollPosition;
    private List<NPC> npcs = new List<NPC>();

    [MenuItem("Game/NPC Creator")]
    public static void ShowWindow()
    {
        GetWindow<NPCWindow>("NPC Creator");
    }

    private void OnEnable()
    {
        LoadNPCs();
    }

    private void LoadNPCs()
    {
        npcs.Clear();
        string[] guids = AssetDatabase.FindAssets("t:NPC");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            NPC npc = AssetDatabase.LoadAssetAtPath<NPC>(path);
            if (npc != null)
            {
                npcs.Add(npc);
            }
        }
    }

    private void DisplayNPCList()
    {
        GUILayout.Label("NPC List", EditorStyles.boldLabel);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < npcs.Count; i++)
        {
            var npc = npcs[i];
            EditorGUILayout.BeginVertical("box");

            string newName = EditorGUILayout.TextField("Name", npc.Name);
            if (newName != npc.Name)
            {
                npc.Name = newName;
                EditorUtility.SetDirty(npc);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUI.enabled = false;
            EditorGUILayout.ObjectField(npc.Prefab, typeof(GameObject), false);
            GUI.enabled = true;

            GUILayout.Space(16);

            GUILayout.Label("Orientations", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Orientation", GUILayout.Width(150)))
            {
                npc.Orientations.Add(FactionType.Traditionalist);
                EditorUtility.SetDirty(npc);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            for (int j = 0; j < npc.Orientations.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                FactionType newOrientation = (FactionType)EditorGUILayout.EnumPopup("Orientation", npc.Orientations[j]);
                if (newOrientation != npc.Orientations[j])
                {
                    npc.Orientations[j] = newOrientation;
                    EditorUtility.SetDirty(npc);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("Delete", GUILayout.Width(100)))
                {
                    npc.Orientations.RemoveAt(j);
                    EditorUtility.SetDirty(npc);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(16);

            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                DeleteNPC(npc);
            }
            EditorGUILayout.EndVertical();
        }

        GUILayout.EndScrollView();
    }

    private void DeleteNPC(NPC npc)
    {
        if (npc == null) return;

        string[] guids = AssetDatabase.FindAssets("t:GameData");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            NPCManager npcManager = AssetDatabase.LoadAssetAtPath<NPCManager>(path);
            if (npcManager != null)
            {
                var interactionsToRemove = npcManager.NPCInteractions.FindAll(interaction => interaction.NPC == npc);
                foreach (var interaction in interactionsToRemove)
                {
                    string interactionPath = AssetDatabase.GetAssetPath(interaction);
                    AssetDatabase.DeleteAsset(interactionPath);
                }
                npcManager.NPCInteractions.RemoveAll(interaction => interaction.NPC == npc);
                EditorUtility.SetDirty(npcManager);
            }
        }

        if (npc.Prefab != null)
        {
            string prefabPath = AssetDatabase.GetAssetPath(npc.Prefab);
            AssetDatabase.DeleteAsset(prefabPath);
        }

        string npcPath = AssetDatabase.GetAssetPath(npc);
        AssetDatabase.DeleteAsset(npcPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        LoadNPCs();
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a new NPC Prefab", EditorStyles.boldLabel);

        npcName = EditorGUILayout.TextField("NPC Name", npcName);
        moveSpeed = EditorGUILayout.FloatField("Move Speed", moveSpeed);
        model3D = (GameObject)EditorGUILayout.ObjectField("3D Model", model3D, typeof(GameObject), false, GUILayout.Width(300));

        GUILayout.Label("Orientations", EditorStyles.boldLabel);

        if (orientations == null)
        {
            orientations = new List<FactionType>();
        }

        if (GUILayout.Button("Add Orientation"))
        {
            orientations.Add(FactionType.Traditionalist);
        }

        for (int i = 0; i < orientations.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            orientations[i] = (FactionType)EditorGUILayout.EnumPopup($"Orientation {i + 1}", orientations[i]);
            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                orientations.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(16);

        if (GUILayout.Button("Create Prefab"))
        {
            var prefab = CreatePrefab();
            CreateNPCScriptableObject(new NPC
            {
                Name = npcName,
                Prefab = prefab,
                Orientations = orientations
            });

            npcName = "";
            model3D = null;
            orientations = new List<FactionType>();
            LoadNPCs();
        }
        
        GUILayout.Space(32);

        DisplayNPCList();
    }

    private GameObject CreatePrefab()
    {
        if (string.IsNullOrEmpty(npcName) || model3D == null)
        {
            Debug.LogError("NPC Name and 3D Model must be set.");
            return null;
        }

        GameObject npcObject = new GameObject(npcName);
        var npcView = npcObject.AddComponent<NPCView>();
        
        GameObject modelInstance = Instantiate(model3D, npcObject.transform);
        modelInstance.name = model3D.name;
        modelInstance.transform.localPosition = Vector3.zero;
        modelInstance.transform.localRotation = Quaternion.identity;
        modelInstance.transform.localScale = Vector3.one;

        Renderer renderer = modelInstance.GetComponent<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            float halfHeight = bounds.size.y / 2;
            modelInstance.transform.localPosition = new Vector3(0, halfHeight, 0);
        }

        string localPath = "Assets/Prefabs/NPCs/" + npcName + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(npcObject, localPath);
        DestroyImmediate(npcObject);

        return AssetDatabase.LoadAssetAtPath<GameObject>(localPath);
    }

    private void CreateNPCScriptableObject(NPC npc)
    {
        if (npc == null)
        {
            Debug.LogError("NPC cannot be null.");
            return;
        }

        string assetPath = "Assets/ScriptableObjects/NPCs/" + npc.Name + ".asset";
        AssetDatabase.CreateAsset(npc, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
