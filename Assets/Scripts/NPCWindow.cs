using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
public class NPCWindow : EditorWindow
{
    private string npcName;
    private GameObject model3D;
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
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

        foreach (var npc in npcs)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(npc.Name, GUILayout.Width(200));
            GUI.enabled = false;
            EditorGUILayout.ObjectField(npc.Prefab, typeof(GameObject), false, GUILayout.Width(100));
            GUI.enabled = true;

            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                DeleteNPC(npc);
                break;
            }
            EditorGUILayout.EndHorizontal();
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
            GameData gameData = AssetDatabase.LoadAssetAtPath<GameData>(path);
            if (gameData != null)
            {
                gameData.NPCInteractions.RemoveAll(interaction => interaction.NPC == npc);
                EditorUtility.SetDirty(gameData);
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

        if (GUILayout.Button("Create Prefab"))
        {
            var prefab = CreatePrefab();
            CreateNPCScriptableObject(new NPC
            {
                Name = npcName,
                Prefab = prefab
            });

            npcName = "";
            model3D = null;
            moveSpeed = 5f;
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
        npcView.MoveSpeed = moveSpeed;
        
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
