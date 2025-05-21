using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

public class LawWindow : EditorWindow
{
    private GameData _gameData;
    private string _newLawName = "";
    private string _newLawDescription = "";
    private List<NPCInteraction> _npcInteractions;
    private Sprite _newLawIcon;
    private List<LawEffect> _newLawEffects;
    private Vector2 _scrollPosition;

    [MenuItem("Game/Law Manager")]
    public static void ShowWindow()
    {
        GetWindow<LawWindow>("Law Manager");
    }

    private void OnEnable()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameData");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _gameData = AssetDatabase.LoadAssetAtPath<GameData>(path);
        }
    }

    private void OnGUI()
    {
        if (_gameData == null)
        {
            EditorGUILayout.LabelField("GameData asset not found.");
            return;
        }

        using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition))
        {
            _scrollPosition = scrollView.scrollPosition;

            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(8);
            EditorGUILayout.LabelField("Create New Law", EditorStyles.boldLabel);
            _newLawName = EditorGUILayout.TextField("Name", _newLawName);
            _newLawDescription = EditorGUILayout.TextField("Description", _newLawDescription);
            _newLawIcon = (Sprite)EditorGUILayout.ObjectField("Icon", _newLawIcon, typeof(Sprite), false);

            if (_npcInteractions == null)
            {
                _npcInteractions = new List<NPCInteraction>();
            }

            EditorGUILayout.LabelField("NPC Interactions", EditorStyles.boldLabel);

            if (GUILayout.Button("Add NPC Interaction"))
            {
                _npcInteractions.Add(new NPCInteraction());
            }

            for (int i = 0; i < _npcInteractions.Count; i++)
            {
                GUILayout.Space(8);

                EditorGUILayout.BeginVertical("box");
                _npcInteractions[i] = (NPCInteraction)EditorGUILayout.ObjectField("NPC Interaction", _npcInteractions[i], typeof(NPCInteraction), false);

                if (GUILayout.Button("Remove NPC Interaction"))
                {
                    _npcInteractions.RemoveAt(i);
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
            if (_newLawEffects == null)
            {
                _newLawEffects = new List<LawEffect>();
            }

            if (GUILayout.Button("Add Effect"))
            {
                _newLawEffects.Add(new LawEffect { Type = FactionType.Traditionalist, Value = 0 });
            }

            for (int i = 0; i < _newLawEffects.Count; i++)
            {
                GUILayout.Space(8);

                EditorGUILayout.BeginVertical("box");
                _newLawEffects[i].Type = (FactionType)EditorGUILayout.EnumPopup("Type", _newLawEffects[i].Type);
                _newLawEffects[i].Value = EditorGUILayout.IntField("Value", _newLawEffects[i].Value);

                if (GUILayout.Button("Remove Effect"))
                {
                    _newLawEffects.RemoveAt(i);
                }
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(16);

            if (GUILayout.Button("Add Law"))
            {
                AddNewLaw();
            }

            GUILayout.Space(8);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Existing Laws", EditorStyles.boldLabel);

            for (int i = 0; i < _gameData.Laws.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");

                string newName = EditorGUILayout.TextField("Name", _gameData.Laws[i].Name);
                if (newName != _gameData.Laws[i].Name)
                {
                    _gameData.Laws[i].Name = newName;
                    EditorUtility.SetDirty(_gameData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                string newDescription = EditorGUILayout.TextField("Description", _gameData.Laws[i].Description);
                if (newDescription != _gameData.Laws[i].Description)
                {
                    _gameData.Laws[i].Description = newDescription;
                    EditorUtility.SetDirty(_gameData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                Sprite newIcon = (Sprite)EditorGUILayout.ObjectField("Icon", _gameData.Laws[i].Icon, typeof(Sprite), false);
                if (newIcon != _gameData.Laws[i].Icon)
                {
                    _gameData.Laws[i].Icon = newIcon;
                    EditorUtility.SetDirty(_gameData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                if (_gameData.Laws[i].NPCInteractions == null)
                {
                    _gameData.Laws[i].NPCInteractions = new List<NPCInteraction>();
                }

                EditorGUILayout.LabelField("NPC Interactions", EditorStyles.boldLabel);

                for (int j = 0; j < _gameData.Laws[i].NPCInteractions.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();

                    NPCInteraction newNPCInteraction = (NPCInteraction)EditorGUILayout.ObjectField("NPC Interaction", _gameData.Laws[i].NPCInteractions[j], typeof(NPCInteraction), false);
                    if (newNPCInteraction != _gameData.Laws[i].NPCInteractions[j])
                    {
                        _gameData.Laws[i].NPCInteractions[j] = newNPCInteraction;
                        EditorUtility.SetDirty(_gameData);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    if (GUILayout.Button("Remove"))
                    {
                        _gameData.Laws[i].NPCInteractions.RemoveAt(j);
                        EditorUtility.SetDirty(_gameData);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add NPC Interaction"))
                {
                    _gameData.Laws[i].NPCInteractions.Add(null);
                    EditorUtility.SetDirty(_gameData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
                if (_gameData.Laws[i].Effects == null)
                {
                    _gameData.Laws[i].Effects = new List<LawEffect>();
                }

                if (GUILayout.Button("Add Effect"))
                {
                    _gameData.Laws[i].Effects.Add(new LawEffect { Type = FactionType.Traditionalist, Value = 0 });
                    EditorUtility.SetDirty(_gameData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                for (int j = 0; j < _gameData.Laws[i].Effects.Count; j++)
                {
                    GUILayout.Space(8);

                    EditorGUILayout.BeginVertical("box");

                    FactionType newType = (FactionType)EditorGUILayout.EnumPopup("Type", _gameData.Laws[i].Effects[j].Type);
                    if (newType != _gameData.Laws[i].Effects[j].Type)
                    {
                        _gameData.Laws[i].Effects[j].Type = newType;
                        EditorUtility.SetDirty(_gameData);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    int newValue = EditorGUILayout.IntField("Value", _gameData.Laws[i].Effects[j].Value);
                    if (newValue != _gameData.Laws[i].Effects[j].Value)
                    {
                        _gameData.Laws[i].Effects[j].Value = newValue;
                        EditorUtility.SetDirty(_gameData);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    if (GUILayout.Button("Remove Effect"))
                    {
                        _gameData.Laws[i].Effects.RemoveAt(j);
                        EditorUtility.SetDirty(_gameData);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(16);

                if (GUILayout.Button("Delete"))
                {
                    _gameData.Laws.RemoveAt(i);
                    EditorUtility.SetDirty(_gameData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                
                EditorGUILayout.EndVertical();

                GUILayout.Space(32);
            }
        }
    }

    private void AddNewLaw()
    {
        if (!string.IsNullOrEmpty(_newLawName) && !string.IsNullOrEmpty(_newLawDescription))
        {
            Law newLaw = new Law
            {
                Name = _newLawName,
                Description = _newLawDescription,
                Icon = _newLawIcon,
                Effects = _newLawEffects,
                NPCInteractions = _npcInteractions,
            };
            _gameData.Laws.Add(newLaw);
            EditorUtility.SetDirty(_gameData);

            _newLawName = "";
            _newLawDescription = "";
            _newLawIcon = null;
            _newLawEffects = null;
            _npcInteractions = null;
        }
    }
}
#endif