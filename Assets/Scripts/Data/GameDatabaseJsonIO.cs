using System.IO;
using UnityEngine;

public static class GameDatabaseJsonIO
{
    private const string JsonPath = "Assets/Resources/GameData/game_database.json";

    public static GameDatabaseRoot Load()
    {
        if (!File.Exists(JsonPath))
            return null;

        var json = File.ReadAllText(JsonPath);
        return JsonUtility.FromJson<GameDatabaseRoot>(json);
    }

    public static void Save(GameDatabaseRoot data)
    {
        var dir = Path.GetDirectoryName(JsonPath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(JsonPath, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
