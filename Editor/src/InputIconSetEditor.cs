using UnityEditor;
using UnityEngine;
using InputPrompts.Runtime;

namespace InputPrompts.Editor
{
    /// <summary>
    /// Custom inspector for InputIconSet. Thin facade: delegates the actual work to
    /// InputIconSetFiller so the logic stays shared with the installer.
    /// </summary>
    [CustomEditor(typeof(InputIconSet))]
    public sealed class InputIconSetEditor : UnityEditor.Editor
    {
        #region Editor GUI

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Auto-fill from sprites", EditorStyles.boldLabel);

            _spriteFolder = (DefaultAsset)EditorGUILayout.ObjectField(
                "Sprite Folder", _spriteFolder, typeof(DefaultAsset), false);

            using (new EditorGUI.DisabledScope(_spriteFolder == null))
            {
                if (GUILayout.Button("Auto-fill controls from folder"))
                {
                    AutoFill();
                }
            }
        }

        #endregion


        #region Tools and Utilities

        private void AutoFill()
        {
            var iconSet = (InputIconSet)target;
            string folderPath = AssetDatabase.GetAssetPath(_spriteFolder);

            InputIconSetFiller.Report report = InputIconSetFiller.Fill(iconSet, folderPath);

            Debug.Log($"[InputPrompts] {iconSet.Family}: filled {report.Matched} controls "
                      + $"from {report.SpritesScanned} sprites.");

            if (report.Missing != null && report.Missing.Count > 0)
            {
                Debug.LogWarning(
                    $"[InputPrompts] {iconSet.Family}: no sprite found for: "
                    + $"{string.Join(", ", report.Missing)}");
            }
        }

        #endregion


        #region Private and Protected

        private DefaultAsset _spriteFolder;

        #endregion
    }
}