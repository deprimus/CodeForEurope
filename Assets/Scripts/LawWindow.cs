#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LawWindow : EditorWindow
{
    private GameDatabaseRoot _database;

    private string _newLawName = "";
    private string _newLawDescription = "";
    private Sprite _newLawIcon;
    private List<LawEffectJson> _newLawEffects;
    private List<string> _newInteractionNames;
    private Vector2 _scrollPosition;

    [MenuItem("Game/Law Manager")]
    public static void ShowWindow()
    {
        GetWindow<LawWindow>("Law Manager");
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

        var interactionNames = _database.interactions.Select(i => i.name).ToArray();

        using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition))
        {
            _scrollPosition = scrollView.scrollPosition;

            DrawNewLawSection(interactionNames);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Existing Laws", EditorStyles.boldLabel);

            for (int i = 0; i < _database.laws.Count; i++)
            {
                DrawExistingLaw(i, interactionNames);
                GUILayout.Space(32);
            }
        }
    }

    private void DrawNewLawSection(string[] allInteractions)
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Space(8);
        EditorGUILayout.LabelField("Create New Law", EditorStyles.boldLabel);
        _newLawName = EditorGUILayout.TextField("Name", _newLawName);
        _newLawDescription = EditorGUILayout.TextField("Description", _newLawDescription);
        _newLawIcon = (Sprite)EditorGUILayout.ObjectField("Icon", _newLawIcon, typeof(Sprite), false);

        if (_newInteractionNames == null)
            _newInteractionNames = new List<string>();

        EditorGUILayout.LabelField("NPC Interactions", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Interaction Slot"))
            _newInteractionNames.Add(allInteractions.Length > 0 ? allInteractions[0] : "");

        for (int i = 0; i < _newInteractionNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            int idx = System.Array.IndexOf(allInteractions, _newInteractionNames[i]);
            if (idx < 0) idx = 0;
            if (allInteractions.Length > 0)
            {
                idx = EditorGUILayout.Popup("Interaction", idx, allInteractions);
                _newInteractionNames[i] = allInteractions[idx];
            }
            if (GUILayout.Button("Remove", GUILayout.Width(80)))
                _newInteractionNames.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
        if (_newLawEffects == null)
            _newLawEffects = new List<LawEffectJson>();

        if (GUILayout.Button("Add Effect"))
            _newLawEffects.Add(new LawEffectJson { type = 0, value = 0 });

        for (int i = 0; i < _newLawEffects.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            _newLawEffects[i].type = (int)(FactionType)EditorGUILayout.EnumPopup("Type", (FactionType)_newLawEffects[i].type);
            _newLawEffects[i].value = EditorGUILayout.IntField("Value", _newLawEffects[i].value);
            if (GUILayout.Button("Remove Effect"))
                _newLawEffects.RemoveAt(i);
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(16);

        if (GUILayout.Button("Add Law"))
        {
            if (!string.IsNullOrEmpty(_newLawName) && !string.IsNullOrEmpty(_newLawDescription))
            {
                var iconPath = "";
                if (_newLawIcon != null)
                    iconPath = SpriteToResourcesPath(_newLawIcon);

                _database.laws.Add(new LawJson
                {
                    name = _newLawName,
                    description = _newLawDescription,
                    iconPath = iconPath,
                    effects = _newLawEffects.Select(e => new LawEffectJson { type = e.type, value = e.value }).ToList(),
                    interactionNames = new List<string>(_newInteractionNames)
                });

                GameDatabaseJsonIO.Save(_database);

                _newLawName = "";
                _newLawDescription = "";
                _newLawIcon = null;
                _newLawEffects = null;
                _newInteractionNames = null;
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawExistingLaw(int i, string[] allInteractions)
    {
        var law = _database.laws[i];

        EditorGUILayout.BeginVertical("box");

        string newName = EditorGUILayout.TextField("Name", law.name);
        if (newName != law.name)
        {
            law.name = newName;
            GameDatabaseJsonIO.Save(_database);
        }

        string newDesc = EditorGUILayout.TextField("Description", law.description);
        if (newDesc != law.description)
        {
            law.description = newDesc;
            GameDatabaseJsonIO.Save(_database);
        }

        Sprite currentIcon = string.IsNullOrEmpty(law.iconPath) ? null : Resources.Load<Sprite>(law.iconPath);
        Sprite newIcon = (Sprite)EditorGUILayout.ObjectField("Icon", currentIcon, typeof(Sprite), false);
        if (newIcon != currentIcon)
        {
            law.iconPath = newIcon != null ? SpriteToResourcesPath(newIcon) : "";
            GameDatabaseJsonIO.Save(_database);
        }

        EditorGUILayout.LabelField("NPC Interactions", EditorStyles.boldLabel);

        for (int j = 0; j < law.interactionNames.Count; j++)
        {
            EditorGUILayout.BeginHorizontal();
            int idx = System.Array.IndexOf(allInteractions, law.interactionNames[j]);
            if (idx < 0) idx = 0;
            if (allInteractions.Length > 0)
            {
                int newIdx = EditorGUILayout.Popup("Interaction", idx, allInteractions);
                if (newIdx != idx)
                {
                    law.interactionNames[j] = allInteractions[newIdx];
                    GameDatabaseJsonIO.Save(_database);
                }
            }
            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                law.interactionNames.RemoveAt(j);
                GameDatabaseJsonIO.Save(_database);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Interaction"))
        {
            law.interactionNames.Add(allInteractions.Length > 0 ? allInteractions[0] : "");
            GameDatabaseJsonIO.Save(_database);
        }

        EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Effect"))
        {
            law.effects.Add(new LawEffectJson { type = 0, value = 0 });
            GameDatabaseJsonIO.Save(_database);
        }

        for (int j = 0; j < law.effects.Count; j++)
        {
            EditorGUILayout.BeginVertical("box");

            var newType = (int)(FactionType)EditorGUILayout.EnumPopup("Type", (FactionType)law.effects[j].type);
            if (newType != law.effects[j].type)
            {
                law.effects[j].type = newType;
                GameDatabaseJsonIO.Save(_database);
            }

            int newValue = EditorGUILayout.IntField("Value", law.effects[j].value);
            if (newValue != law.effects[j].value)
            {
                law.effects[j].value = newValue;
                GameDatabaseJsonIO.Save(_database);
            }

            if (GUILayout.Button("Remove Effect"))
            {
                law.effects.RemoveAt(j);
                GameDatabaseJsonIO.Save(_database);
            }
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(16);

        if (GUILayout.Button("Delete Law"))
        {
            _database.laws.RemoveAt(i);
            GameDatabaseJsonIO.Save(_database);
        }

        EditorGUILayout.EndVertical();
    }

    private string SpriteToResourcesPath(Sprite sprite)
    {
        var path = AssetDatabase.GetAssetPath(sprite);
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
