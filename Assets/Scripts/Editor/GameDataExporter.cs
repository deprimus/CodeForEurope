#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class GameDataExporter
{
    private const string JsonOutputPath = "Assets/Resources/GameData/game_database.json";
    private const string ResourcesPrefabDir = "Assets/Resources/Prefabs/NPCs";

    [MenuItem("Game/Export All to JSON (Full)")]
    public static void FullExport()
    {
        CopyNPCPrefabsToResources();
        Export();
        Debug.Log("GameDataExporter: Full export complete. Prefabs copied and JSON written.");
    }

    [MenuItem("Game/Export ScriptableObjects to JSON")]
    public static void Export()
    {
        var root = new GameDatabaseRoot
        {
            npcs = new List<NpcJson>(),
            interactions = new List<InteractionJson>(),
            laws = new List<LawJson>()
        };

        var npcIdMap = ExportNPCs(root);
        ExportInteractions(root, npcIdMap);
        ExportLaws(root);

        EnsureAssetDirectoryExists("Assets/Resources/GameData");
        var json = JsonUtility.ToJson(root, true);
        File.WriteAllText(JsonOutputPath, json);
        AssetDatabase.Refresh();

        Debug.Log($"GameDataExporter: Exported to {JsonOutputPath}");
        Debug.Log($"  {root.npcs.Count} NPCs, {root.interactions.Count} interactions, {root.laws.Count} laws");
    }

    [MenuItem("Game/Delete ScriptableObject Assets")]
    public static void DeleteAllScriptableObjects()
    {
        var foldersToDelete = new[]
        {
            "Assets/ScriptableObjects/NPCs",
            "Assets/ScriptableObjects/NPCInteractions"
        };

        var filesToDelete = new[]
        {
            "Assets/ScriptableObjects/GameData.asset",
            "Assets/ScriptableObjects/NPCManager.asset",
            "Assets/ScriptableObjects/LawManager.asset"
        };

        foreach (var file in filesToDelete)
        {
            if (File.Exists(file))
            {
                AssetDatabase.DeleteAsset(file);
                Debug.Log($"Deleted {file}");
            }
        }

        foreach (var folder in foldersToDelete)
        {
            if (Directory.Exists(folder))
            {
                AssetDatabase.DeleteAsset(folder);
                Debug.Log($"Deleted folder {folder}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("GameDataExporter: All ScriptableObject assets deleted.");
    }

    [MenuItem("Game/Copy NPC Prefabs to Resources")]
    public static void CopyNPCPrefabsToResources()
    {
        EnsureAssetDirectoryExists(ResourcesPrefabDir);

        var guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/NPCs" });
        if (guids.Length == 0)
        {
            Debug.Log("No prefabs found in Assets/Prefabs/NPCs/");
            return;
        }

        foreach (var guid in guids)
        {
            var srcPath = AssetDatabase.GUIDToAssetPath(guid);
            var fileName = Path.GetFileName(srcPath);
            var dstPath = $"{ResourcesPrefabDir}/{fileName}";

            if (AssetDatabase.LoadAssetAtPath<Object>(dstPath) != null)
            {
                Debug.Log($"Prefab already exists at {dstPath}, skipping.");
                continue;
            }

            if (AssetDatabase.CopyAsset(srcPath, dstPath))
                Debug.Log($"Copied {srcPath} -> {dstPath}");
            else
                Debug.LogError($"Failed to copy {srcPath} -> {dstPath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static Dictionary<NPC, string> ExportNPCs(GameDatabaseRoot root)
    {
        var npcIdMap = new Dictionary<NPC, string>();
        var guids = AssetDatabase.FindAssets("t:NPC");

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var npc = AssetDatabase.LoadAssetAtPath<NPC>(path);
            if (npc == null) continue;

            var id = npc.name;
            npcIdMap[npc] = id;

            var prefabPath = "";
            if (npc.Prefab != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(npc.Prefab);
                prefabPath = AssetPathToResourcesPath(assetPath);
            }

            root.npcs.Add(new NpcJson
            {
                id = id,
                name = npc.Name,
                prefabPath = prefabPath,
                orientations = npc.Orientations?.Select(o => (int)o).ToList() ?? new List<int>()
            });
        }

        return npcIdMap;
    }

    private static void ExportInteractions(GameDatabaseRoot root, Dictionary<NPC, string> npcIdMap)
    {
        var guids = AssetDatabase.FindAssets("t:NPCInteraction");

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var interaction = AssetDatabase.LoadAssetAtPath<NPCInteraction>(path);
            if (interaction == null) continue;

            var npcId = "";
            if (interaction.NPC != null && npcIdMap.ContainsKey(interaction.NPC))
                npcId = npcIdMap[interaction.NPC];
            else if (interaction.NPC != null)
                npcId = interaction.NPC.name;

            root.interactions.Add(new InteractionJson
            {
                name = interaction.Name,
                npcId = npcId,
                dialogue = interaction.Dialogue ?? new List<string>(),
                effects = interaction.Effects?.Select(e => new InteractionEffectJson
                {
                    type = (int)e.Type,
                    value = e.Value
                }).ToList() ?? new List<InteractionEffectJson>()
            });
        }
    }

    private static void ExportLaws(GameDatabaseRoot root)
    {
        var guids = AssetDatabase.FindAssets("t:GameData");

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var gameData = AssetDatabase.LoadAssetAtPath<GameData>(path);
            if (gameData == null || gameData.Laws == null) continue;

            foreach (var law in gameData.Laws)
            {
                var iconPath = "";
                if (law.Icon != null)
                {
                    var assetPath = AssetDatabase.GetAssetPath(law.Icon);
                    iconPath = AssetPathToResourcesPath(assetPath);
                }

                root.laws.Add(new LawJson
                {
                    name = law.Name,
                    description = law.Description,
                    iconPath = iconPath,
                    effects = law.Effects?.Select(e => new LawEffectJson
                    {
                        type = (int)e.Type,
                        value = e.Value
                    }).ToList() ?? new List<LawEffectJson>(),
                    interactionNames = law.NPCInteractions?
                        .Where(i => i != null)
                        .Select(i => i.Name)
                        .ToList() ?? new List<string>()
                });
            }
        }
    }

    private static string AssetPathToResourcesPath(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath)) return "";

        const string resourcesPrefix = "Assets/Resources/";
        if (assetPath.StartsWith(resourcesPrefix))
        {
            var relative = assetPath.Substring(resourcesPrefix.Length);
            var ext = Path.GetExtension(relative);
            if (!string.IsNullOrEmpty(ext))
                relative = relative.Substring(0, relative.Length - ext.Length);
            return relative;
        }

        if (assetPath.StartsWith("Assets/Prefabs/NPCs/"))
        {
            var fileName = Path.GetFileNameWithoutExtension(assetPath);
            return $"Prefabs/NPCs/{fileName}";
        }

        Debug.LogWarning($"Asset '{assetPath}' is not under Resources/ — it won't be loadable at runtime.");
        return assetPath;
    }

    /// <summary>
    /// Creates a folder path using AssetDatabase so Unity properly tracks each directory.
    /// </summary>
    private static void EnsureAssetDirectoryExists(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        var parts = path.Split('/');
        var current = parts[0]; // "Assets"

        for (int i = 1; i < parts.Length; i++)
        {
            var next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}
#endif
