using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using InputPrompts.Runtime;

namespace InputPrompts.Editor
{
    /// <summary>
    /// Shared auto-fill logic for an InputIconSet (used by the inspector and the
    /// installer). Loads sprites from a folder, matches them against the family's
    /// convention table, and writes the Controls table via SerializedObject.
    /// </summary>
    public static class InputIconSetFiller
    {
        #region Public API

        public struct Report
        {
            public int Matched;
            public int SpritesScanned;
            public List<string> Missing;
        }

        public static Report Fill(InputIconSet iconSet, string spriteFolderPath)
        {
            var report = new Report { Missing = new List<string>() };

            if (iconSet == null || !AssetDatabase.IsValidFolder(spriteFolderPath))
            {
                return report;
            }

            (string control, string[] candidates)[] table = IconConventions.ForFamily(iconSet.Family);
            if (table == null)
            {
                return report;
            }

            Dictionary<string, Sprite> sprites = LoadSprites(spriteFolderPath);
            report.SpritesScanned = sprites.Count;

            var so = new SerializedObject(iconSet);
            SerializedProperty controls = so.FindProperty("_controls");
            controls.ClearArray();

            for (int i = 0; i < table.Length; i++)
            {
                Sprite sprite = FindSprite(sprites, table[i].candidates);
                if (sprite == null)
                {
                    report.Missing.Add(table[i].control);
                    continue;
                }

                controls.InsertArrayElementAtIndex(report.Matched);
                SerializedProperty element = controls.GetArrayElementAtIndex(report.Matched);
                element.FindPropertyRelative("controlPath").stringValue = table[i].control;
                element.FindPropertyRelative("sprite").objectReferenceValue = sprite;
                report.Matched++;
            }

            so.ApplyModifiedProperties();
            return report;
        }

        #endregion


        #region Tools and Utilities

        public static Dictionary<string, Sprite> LoadSprites(string folderPath)
        {
            var result = new Dictionary<string, Sprite>();
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                for (int j = 0; j < assets.Length; j++)
                {
                    if (assets[j] is Sprite sprite && !result.ContainsKey(sprite.name))
                    {
                        result.Add(sprite.name, sprite);
                    }
                }
            }

            return result;
        }

        // Exact name first; otherwise the shortest name that starts with a candidate
        // (so "xbox_button_color_a" wins over "xbox_button_color_a_outline").
        public static Sprite FindSprite(Dictionary<string, Sprite> sprites, string[] candidates)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                if (sprites.TryGetValue(candidates[i], out Sprite exact))
                {
                    return exact;
                }
            }

            Sprite best = null;
            foreach (KeyValuePair<string, Sprite> kv in sprites)
            {
                for (int i = 0; i < candidates.Length; i++)
                {
                    if (kv.Key.StartsWith(candidates[i])
                        && (best == null || kv.Key.Length < best.name.Length))
                    {
                        best = kv.Value;
                    }
                }
            }

            return best;
        }

        #endregion
    }
}