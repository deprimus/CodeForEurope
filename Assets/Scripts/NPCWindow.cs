#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class NPCWindow : EditorWindow
{
    private const string JsonPath = "Assets/Resources/GameData/game_database.json";

    private string npcName;
    private GameObject model3D;
    private List<FactionType> orientations = new List<FactionType>();

    private Vector2 scrollPosition;
    private GameDatabaseRoot _database;

    [MenuItem("Game/NPC Creator")]
    public static void ShowWindow()
    {
        GetWindow<NPCWindow>("NPC Creator");
    }

    private void OnEnable()
    {
        LoadDatabase();
    }

    private void LoadDatabase()
    {
        _database = GameDatabaseJsonIO.Load();
    }

    private void OnGUI()
    {
        if (_database == null)
        {
            EditorGUILayout.LabelField("No game_database.json found. Use Game > Export ScriptableObjects to JSON first.");
            if (GUILayout.Button("Reload"))
                LoadDatabase();
            return;
        }

        GUILayout.Label("Create a new NPC", EditorStyles.boldLabel);

        npcName = EditorGUILayout.TextField("NPC Name", npcName);
        model3D = (GameObject)EditorGUILayout.ObjectField("3D Model", model3D, typeof(GameObject), false, GUILayout.Width(300));

        GUILayout.Label("Orientations", EditorStyles.boldLabel);

        if (orientations == null)
            orientations = new List<FactionType>();

        if (GUILayout.Button("Add Orientation"))
            orientations.Add(FactionType.Traditionalist);

        for (int i = 0; i < orientations.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            orientations[i] = (FactionType)EditorGUILayout.EnumPopup($"Orientation {i + 1}", orientations[i]);
            if (GUILayout.Button("Delete", GUILayout.Width(100)))
                orientations.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(16);

        if (GUILayout.Button("Create NPC"))
        {
            if (string.IsNullOrEmpty(npcName) || model3D == null)
            {
                Debug.LogError("NPC Name and 3D Model must be set.");
            }
            else
            {
                var prefab = CreatePrefab();
                var prefabPath = GetResourcesPath(prefab);

                _database.npcs.Add(new NpcJson
                {
                    id = npcName,
                    name = npcName,
                    prefabPath = prefabPath,
                    orientations = orientations.Select(o => (int)o).ToList()
                });

                GameDatabaseJsonIO.Save(_database);

                npcName = "";
                model3D = null;
                orientations = new List<FactionType>();
            }
        }

        GUILayout.Space(32);
        DisplayNPCList();
    }

    private void DisplayNPCList()
    {
        GUILayout.Label("NPC List", EditorStyles.boldLabel);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < _database.npcs.Count; i++)
        {
            var npc = _database.npcs[i];
            EditorGUILayout.BeginVertical("box");

            string newName = EditorGUILayout.TextField("Display Name", npc.name);
            if (newName != npc.name)
            {
                npc.name = newName;
                GameDatabaseJsonIO.Save(_database);
            }

            EditorGUILayout.LabelField("ID", npc.id);
            EditorGUILayout.LabelField("Prefab", npc.prefabPath);

            GUILayout.Space(8);
            GUILayout.Label("Orientations", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Orientation", GUILayout.Width(150)))
            {
                npc.orientations.Add(0);
                GameDatabaseJsonIO.Save(_database);
            }

            for (int j = 0; j < npc.orientations.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                var newOrientation = (FactionType)EditorGUILayout.EnumPopup("Orientation", (FactionType)npc.orientations[j]);
                if ((int)newOrientation != npc.orientations[j])
                {
                    npc.orientations[j] = (int)newOrientation;
                    GameDatabaseJsonIO.Save(_database);
                }
                if (GUILayout.Button("Delete", GUILayout.Width(100)))
                {
                    npc.orientations.RemoveAt(j);
                    GameDatabaseJsonIO.Save(_database);
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(8);

            if (GUILayout.Button("Delete NPC", GUILayout.Width(100)))
            {
                _database.interactions.RemoveAll(it => it.npcId == npc.id);

                foreach (var law in _database.laws)
                {
                    var interactionNames = _database.interactions
                        .Where(it => it.npcId == npc.id)
                        .Select(it => it.name)
                        .ToHashSet();
                    law.interactionNames.RemoveAll(n => interactionNames.Contains(n));
                }

                _database.npcs.RemoveAt(i);
                GameDatabaseJsonIO.Save(_database);
            }

            EditorGUILayout.EndVertical();
        }

        GUILayout.EndScrollView();
    }

    private GameObject CreatePrefab()
    {
        GameObject npcObject = new GameObject(npcName);
        npcObject.AddComponent<NPCView>();

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

        string dir = "Assets/Resources/Prefabs/NPCs";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string localPath = $"{dir}/{npcName}.prefab";
        PrefabUtility.SaveAsPrefabAsset(npcObject, localPath);
        DestroyImmediate(npcObject);

        return AssetDatabase.LoadAssetAtPath<GameObject>(localPath);
    }

    private string GetResourcesPath(GameObject prefab)
    {
        if (prefab == null) return "";
        var path = AssetDatabase.GetAssetPath(prefab);
        const string prefix = "Assets/Resources/";
        if (path.StartsWith(prefix))
        {
            var relative = path.Substring(prefix.Length);
            var ext = Path.GetExtension(relative);
            if (!string.IsNullOrEmpty(ext))
                relative = relative.Substring(0, relative.Length - ext.Length);
            return relative;
        }
        return path;
    }
}
#endif
