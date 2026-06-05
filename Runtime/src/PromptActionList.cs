using UnityEngine;
using UnityEngine.InputSystem;

namespace InputPrompts.Runtime
{
    /// <summary>
    /// Data for the persistent legend (Mad Max style: STRIKE / PARRY / COUNTER).
    /// Fully data-driven: the list of displayed actions lives here, not in code.
    /// Reconfigurable per project without touching code.
    /// </summary>
    [CreateAssetMenu(
        fileName = "PromptActionList",
        menuName = "Input Prompts/Prompt Action List")]
    public sealed class PromptActionList : ScriptableObject
    {
        #region Nested Types

        [System.Serializable]
        public struct Entry
        {
            public InputActionReference action;
            [Tooltip("Displayed text (e.g. STRIKE). Empty = action name.")]
            public string label;
        }

        #endregion


        #region Public API

        public int Count => _entries.Length;

        public Entry Get(int index) => _entries[index];

        #endregion


        #region Show In Inspector

        [Header("Entries")]
        [SerializeField] private Entry[] _entries = new Entry[0];

        #endregion
    }
}