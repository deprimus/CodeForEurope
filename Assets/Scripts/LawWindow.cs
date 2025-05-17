using UnityEngine;
using UnityEditor;

public class LawWindow : EditorWindow
{
    private GameData _gameData;
    private string _newLawName = "";
    private string _newLawDescription = "";
    private Sprite _newLawIcon;

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

        EditorGUILayout.BeginVertical("box");
        GUILayout.Space(8);
        EditorGUILayout.LabelField("Create New Law", EditorStyles.boldLabel);
        _newLawName = EditorGUILayout.TextField("Name", _newLawName);
        _newLawDescription = EditorGUILayout.TextField("Description", _newLawDescription);
        _newLawIcon = (Sprite)EditorGUILayout.ObjectField("Icon", _newLawIcon, typeof(Sprite), false);

        if (GUILayout.Button("Add Law"))
        {
            AddNewLaw();
        }

        GUILayout.Space(8);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Existing Laws", EditorStyles.boldLabel);

        using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition, GUILayout.Height(400)))
        {
            _scrollPosition = scrollView.scrollPosition;

            for (int i = 0; i < _gameData.Laws.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                _gameData.Laws[i].Name = EditorGUILayout.TextField("Name", _gameData.Laws[i].Name);
                _gameData.Laws[i].Description = EditorGUILayout.TextField("Description", _gameData.Laws[i].Description);
                _gameData.Laws[i].Icon = (Sprite)EditorGUILayout.ObjectField("Icon", _gameData.Laws[i].Icon, typeof(Sprite), false);

                if (GUILayout.Button("Delete"))
                {
                    _gameData.Laws.RemoveAt(i);
                    EditorUtility.SetDirty(_gameData);
                }
                
                EditorGUILayout.EndVertical();
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
                Icon = _newLawIcon
            };
            _gameData.Laws.Add(newLaw);
            EditorUtility.SetDirty(_gameData);

            _newLawName = "";
            _newLawDescription = "";
            _newLawIcon = null;
        }
    }
}
