using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using InputPrompts.Runtime;

namespace InputPrompts.Editor
{
    /// <summary>
    /// Shared generation logic for a PromptActionList (used by the inspector and the
    /// installer). Reads the InputActionReference sub-assets of an .inputactions asset
    /// and writes one entry per action (label = action name) via SerializedObject.
    /// </summary>
    public static class PromptActionListGenerator
    {
        #region Public API

        public static int Generate(
            PromptActionList list, InputActionAsset asset, bool skipUiMap, bool buttonsOnly)
        {
            if (list == null || asset == null)
            {
                return 0;
            }

            Dictionary<string, InputActionReference> byId = BuildReferenceMap(asset);

            var so = new SerializedObject(list);
            SerializedProperty entries = so.FindProperty("_entries");
            entries.ClearArray();

            int count = 0;
            foreach (InputActionMap map in asset.actionMaps)
            {
                if (skipUiMap && map.name == "UI")
                {
                    continue;
                }

                foreach (InputAction action in map.actions)
                {
                    if (buttonsOnly && action.type != InputActionType.Button)
                    {
                        continue;
                    }

                    if (!byId.TryGetValue(action.id.ToString(), out InputActionReference reference))
                    {
                        continue;
                    }

                    entries.InsertArrayElementAtIndex(count);
                    SerializedProperty element = entries.GetArrayElementAtIndex(count);
                    element.FindPropertyRelative("action").objectReferenceValue = reference;
                    element.FindPropertyRelative("label").stringValue = action.name;
                    count++;
                }
            }

            so.ApplyModifiedProperties();
            return count;
        }

        #endregion


        #region Tools and Utilities

        // The .inputactions asset stores one InputActionReference sub-asset per action.
        public static Dictionary<string, InputActionReference> BuildReferenceMap(InputActionAsset asset)
        {
            var map = new Dictionary<string, InputActionReference>();
            string path = AssetDatabase.GetAssetPath(asset);
            Object[] all = AssetDatabase.LoadAllAssetsAtPath(path);

            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] is InputActionReference reference && reference.action != null)
                {
                    string id = reference.action.id.ToString();
                    if (!map.ContainsKey(id))
                    {
                        map.Add(id, reference);
                    }
                }
            }

            return map;
        }

        #endregion
    }
}