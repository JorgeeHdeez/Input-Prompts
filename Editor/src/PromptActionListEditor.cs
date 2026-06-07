using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using InputPrompts.Runtime;

namespace InputPrompts.Editor
{
    /// <summary>
    /// Custom inspector for PromptActionList. Thin facade: delegates the actual work
    /// to PromptActionListGenerator so the logic stays shared with the installer.
    /// </summary>
    [CustomEditor(typeof(PromptActionList))]
    public sealed class PromptActionListEditor : UnityEditor.Editor
    {
        #region Editor GUI

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_inputActions == null)
            {
                _inputActions = InferAsset();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Auto-generation", EditorStyles.boldLabel);

            _inputActions = (InputActionAsset)EditorGUILayout.ObjectField(
                "Input Actions", _inputActions, typeof(InputActionAsset), false);

            _skipUiMap = EditorGUILayout.Toggle("Skip 'UI' map", _skipUiMap);
            _buttonsOnly = EditorGUILayout.Toggle("Buttons only", _buttonsOnly);

            using (new EditorGUI.DisabledScope(_inputActions == null))
            {
                if (GUILayout.Button("Generate entries from Input Actions"))
                {
                    Generate();
                }
            }
        }

        #endregion


        #region Tools and Utilities

        private void Generate()
        {
            bool ok = EditorUtility.DisplayDialog(
                "Generate entries",
                "This replaces the current entries with one per action from the asset. Continue?",
                "Generate", "Cancel");
            if (!ok)
            {
                return;
            }

            int count = PromptActionListGenerator.Generate(
                (PromptActionList)target, _inputActions, _skipUiMap, _buttonsOnly);

            Debug.Log($"[InputPrompts] Generated {count} entries from {_inputActions.name}.");
        }

        private InputActionAsset InferAsset()
        {
            SerializedProperty entries = serializedObject.FindProperty("_entries");
            for (int i = 0; i < entries.arraySize; i++)
            {
                SerializedProperty actionProp =
                    entries.GetArrayElementAtIndex(i).FindPropertyRelative("action");

                if (actionProp.objectReferenceValue is InputActionReference reference
                    && reference.asset != null)
                {
                    return reference.asset;
                }
            }

            return null;
        }

        #endregion


        #region Private and Protected

        private InputActionAsset _inputActions;
        private bool _skipUiMap = true;
        private bool _buttonsOnly = false;

        #endregion
    }
}