using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using InputPrompts.Runtime;

namespace InputPrompts.Editor
{
    /// <summary>
    /// One-click installer for the Input Prompts feature.
    ///
    /// On Install it:
    ///  1) creates (or reuses) the ScriptableObjects (channel, 3 icon sets, resolver,
    ///     action list) - safe to re-run, it does not duplicate existing assets,
    ///  2) pre-fills the icon sets from a sprite folder and generates a curated action
    ///     list from an .inputactions asset (via the shared helpers),
    ///  3) wires the icon sets into the resolver,
    ///  4) instantiates the legend prefab and a tracker GameObject into the open scene,
    ///     and wires every SO reference.
    ///
    /// The UI is NOT built by code: it comes from your legend prefab (UI_Canvas with
    /// InputLegendView, pointing at your Row prefab). The installer only instantiates
    /// and connects it.
    /// </summary>
    public sealed class InputPromptsInstaller : EditorWindow
    {
        #region Editor GUI

        [MenuItem("Tools/Input Prompts/Installer")]
        public static void ShowWindow()
        {
            InputPromptsInstaller window = GetWindow<InputPromptsInstaller>("Input Prompts Installer");
            window.minSize = new Vector2(440, 400);
        }

        private void OnGUI()
        {
            GUILayout.Label("Inputs", EditorStyles.boldLabel);

            _spriteFolder = (DefaultAsset)EditorGUILayout.ObjectField(
                "Sprite Folder", _spriteFolder, typeof(DefaultAsset), false);
            _inputActions = (InputActionAsset)EditorGUILayout.ObjectField(
                "Input Actions", _inputActions, typeof(InputActionAsset), false);
            _legendPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Legend Prefab (InputPrompts_Canvas)", _legendPrefab, typeof(GameObject), false);
            _outputFolder = EditorGUILayout.TextField("SO Output Folder", _outputFolder);

            EditorGUILayout.Space(6);
            GUILayout.Label("Options", EditorStyles.boldLabel);
            _skipUiMap = EditorGUILayout.Toggle("Skip 'UI' map", _skipUiMap);
            _buttonsOnly = EditorGUILayout.Toggle("Buttons only", _buttonsOnly);
            _curate = EditorGUILayout.Toggle("Curate (skip Move/Look/...)", _curate);
            _addToScene = EditorGUILayout.Toggle("Add to open scene", _addToScene);

            EditorGUILayout.Space(10);
            using (new EditorGUI.DisabledScope(!CanInstall()))
            {
                if (GUILayout.Button("Install", GUILayout.Height(32)))
                {
                    Install();
                }
            }

            if (!CanInstall())
            {
                EditorGUILayout.HelpBox(
                    "Set the sprite folder, the .inputactions asset and the legend prefab.",
                    MessageType.Info);
            }
        }

        private bool CanInstall()
        {
            return _spriteFolder != null
                && _inputActions != null
                && _legendPrefab != null
                && !string.IsNullOrEmpty(_outputFolder);
        }

        #endregion


        #region Install

        private void Install()
        {
            CheckTmpResources();

            string folder = EnsureFolder(_outputFolder);
            string spriteFolderPath = AssetDatabase.GetAssetPath(_spriteFolder);

            var channel = GetOrCreate<InputDeviceChangedChannel>(folder, "InputDeviceChangedChannel");
            InputIconSet keyboard = GetOrCreateIconSet(folder, "IconSet_Keyboard", DeviceFamily.KeyboardMouse);
            InputIconSet xbox = GetOrCreateIconSet(folder, "IconSet_Xbox", DeviceFamily.Xbox);
            InputIconSet playstation = GetOrCreateIconSet(folder, "IconSet_PlayStation", DeviceFamily.PlayStation);
            var resolver = GetOrCreate<InputIconResolver>(folder, "InputIconResolver");
            var actionList = GetOrCreate<PromptActionList>(folder, "PromptActionList");

            FillIconSet(keyboard, spriteFolderPath);
            FillIconSet(xbox, spriteFolderPath);
            FillIconSet(playstation, spriteFolderPath);

            int entries = PromptActionListGenerator.Generate(
                actionList, _inputActions, _skipUiMap, _buttonsOnly,
                _curate ? PromptActionListGenerator.DefaultExclusions : System.Array.Empty<string>());
            Debug.Log($"[InputPrompts] Generated {entries} legend entries.");

            WireResolver(resolver, new[] { keyboard, xbox, playstation });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (_addToScene)
            {
                AddToScene(channel, resolver, actionList);
            }

            Debug.Log("[InputPrompts] Install complete.");
            LogReminders(_addToScene);
        }

        private void AddToScene(
            InputDeviceChangedChannel channel, InputIconResolver resolver, PromptActionList actionList)
        {
            InputLegendView existingView =
                FindFirstObjectByType<InputLegendView>(FindObjectsInactive.Include);
            InputDeviceTracker existingTracker =
                FindFirstObjectByType<InputDeviceTracker>(FindObjectsInactive.Include);

            if (existingView != null || existingTracker != null)
            {
                Debug.LogWarning("[InputPrompts] A legend or tracker already exists in the scene. "
                    + "Skipped scene setup to avoid duplicates.");
                return;
            }

            var canvas = (GameObject)PrefabUtility.InstantiatePrefab(_legendPrefab);
            Undo.RegisterCreatedObjectUndo(canvas, "Add Input Prompts Legend");

            InputLegendView view = canvas.GetComponentInChildren<InputLegendView>(true);
            if (view != null)
            {
                var viewSo = new SerializedObject(view);
                viewSo.FindProperty("_channel").objectReferenceValue = channel;
                viewSo.FindProperty("_resolver").objectReferenceValue = resolver;
                viewSo.FindProperty("_actionList").objectReferenceValue = actionList;
                viewSo.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogWarning("[InputPrompts] No InputLegendView found on the legend prefab.");
            }

            var trackerGo = new GameObject("InputDeviceTracker");
            Undo.RegisterCreatedObjectUndo(trackerGo, "Add Input Device Tracker");
            InputDeviceTracker tracker = trackerGo.AddComponent<InputDeviceTracker>();

            var trackerSo = new SerializedObject(tracker);
            trackerSo.FindProperty("_channel").objectReferenceValue = channel;
            trackerSo.ApplyModifiedProperties();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static void LogReminders(bool addedToScene)
        {
            Debug.LogWarning("[InputPrompts] TODO 1/2 - Composite: add '2DVector' -> your WASD "
                + "cluster sprite in IconSet_Keyboard > Composites. The installer cannot guess it.");

            Debug.LogWarning("[InputPrompts] TODO 2/2 - Review PromptActionList: keep only the verbs "
                + "you want in the corner (curation already skipped Move/Look/... if enabled).");

            if (addedToScene)
            {
                Debug.LogWarning("[InputPrompts] Scene - If your legend prefab contains an EventSystem, "
                    + "delete it. The scene already has one (avoids 'Multiple EventSystems').");

                Debug.LogWarning("[InputPrompts] Scene - The legend uses its own Canvas. If it renders "
                    + "behind other UI, raise that Canvas' Sort Order.");
            }
        }

        #endregion


        #region Tools and Utilities

        private static void CheckTmpResources()
        {
            string[] found = AssetDatabase.FindAssets("t:TMP_Settings");
            if (found == null || found.Length == 0)
            {
                Debug.LogWarning("[InputPrompts] TextMeshPro resources not found. The legend text "
                    + "will not render. Import them via Window > TextMeshPro > Import TMP Essential "
                    + "Resources.");
            }
        }

        private static void FillIconSet(InputIconSet set, string spriteFolderPath)
        {
            InputIconSetFiller.Report report = InputIconSetFiller.Fill(set, spriteFolderPath);

            Debug.Log($"[InputPrompts] {set.Family}: filled {report.Matched} controls "
                + $"from {report.SpritesScanned} sprites.");

            if (report.Missing != null && report.Missing.Count > 0)
            {
                Debug.LogWarning($"[InputPrompts] {set.Family}: no sprite found for: "
                    + $"{string.Join(", ", report.Missing)}");
            }
        }

        private static InputIconSet GetOrCreateIconSet(string folder, string assetName, DeviceFamily family)
        {
            InputIconSet set = GetOrCreate<InputIconSet>(folder, assetName);

            var so = new SerializedObject(set);
            so.FindProperty("_family").enumValueIndex = (int)family;
            so.ApplyModifiedProperties();

            return set;
        }

        private static void WireResolver(InputIconResolver resolver, InputIconSet[] sets)
        {
            var so = new SerializedObject(resolver);
            SerializedProperty array = so.FindProperty("_iconSets");
            array.ClearArray();

            for (int i = 0; i < sets.Length; i++)
            {
                array.InsertArrayElementAtIndex(i);
                array.GetArrayElementAtIndex(i).objectReferenceValue = sets[i];
            }

            so.ApplyModifiedProperties();
        }

        private static T GetOrCreate<T>(string folder, string assetName) where T : ScriptableObject
        {
            string path = $"{folder}/{assetName}.asset";
            T existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                return existing;
            }

            T asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static string EnsureFolder(string projectPath)
        {
            if (AssetDatabase.IsValidFolder(projectPath))
            {
                return projectPath;
            }

            string[] parts = projectPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }

            return current;
        }

        #endregion


        #region Private and Protected

        private DefaultAsset _spriteFolder;
        private InputActionAsset _inputActions;
        private GameObject _legendPrefab;
        private string _outputFolder = "Assets/_/Database/ScriptableObject/InputPrompts";

        private bool _skipUiMap = true;
        private bool _buttonsOnly = false;
        private bool _curate = true;
        private bool _addToScene = true;

        #endregion
    }
}
