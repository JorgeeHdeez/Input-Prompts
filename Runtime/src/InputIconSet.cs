using System.Collections.Generic;
using UnityEngine;

namespace InputPrompts.Runtime
{
    /// <summary>
    /// Icon set for ONE device family (Xbox, PlayStation, Keyboard...).
    /// This is where the artist plugs the Kenney sprites.
    ///
    /// Two tables:
    ///  - _controls   : SIMPLE binding -> sprite. Key = control name
    ///                  (e.g. "buttonSouth", "space", "leftTrigger", "w").
    ///  - _composites : COMPOSITE binding -> 1 glyph (Option A). Key = composite
    ///                  type (e.g. "2DVector", "Dpad", "1DAxis").
    ///                  E.g. keyboard "2DVector" -> WASD cluster; pad -> left stick.
    /// </summary>
    [CreateAssetMenu(
        fileName = "InputIconSet",
        menuName = "Input Prompts/Icon Set")]
    public sealed class InputIconSet : ScriptableObject
    {
        #region Unity API

        public void OnEnable()
        {
            BuildMaps();
        }

        #endregion


        #region Public API

        public DeviceFamily Family => _family;

        public bool TryGetControl(string controlPath, out Sprite sprite)
        {
            if (_controlMap == null) BuildMaps();

            return _controlMap.TryGetValue(Normalize(controlPath), out sprite);
        }

        public bool TryGetComposite(string compositeType, out Sprite sprite)
        {
            if (_compositeMap == null) BuildMaps();

            return _compositeMap.TryGetValue(Normalize(compositeType), out sprite);
        }

        #endregion


        #region Tools and Utilities

        private void BuildMaps()
        {
            _controlMap = new Dictionary<string, Sprite>(_controls.Length);
            for (int i = 0; i < _controls.Length; i++)
            {
                string key = Normalize(_controls[i].controlPath);
                if (key.Length > 0 && !_controlMap.ContainsKey(key))
                {
                    _controlMap.Add(key, _controls[i].sprite);
                }
            }

            _compositeMap = new Dictionary<string, Sprite>(_composites.Length);
            for (int i = 0; i < _composites.Length; i++)
            {
                string key = Normalize(_composites[i].compositeType);
                if (key.Length > 0 && !_compositeMap.ContainsKey(key))
                {
                    _compositeMap.Add(key, _composites[i].sprite);
                }
            }
        }

        private static string Normalize(string raw)
        {
            return string.IsNullOrEmpty(raw) ? string.Empty : raw.Trim().ToLowerInvariant();
        }

        #endregion


        #region Show In Inspector

        [System.Serializable]
        private struct ControlIcon
        {
            [Tooltip("Control name. E.g. buttonSouth, buttonEast, space, leftTrigger, w")]
            public string controlPath;
            public Sprite sprite;
        }

        [System.Serializable]
        private struct CompositeIcon
        {
            [Tooltip("Composite type. E.g. 2DVector, Dpad, 1DAxis")]
            public string compositeType;
            [Tooltip("Single glyph (Option A): WASD cluster, left stick, d-pad...")]
            public Sprite sprite;
        }

        [Header("Family")]
        [SerializeField] private DeviceFamily _family = DeviceFamily.KeyboardMouse;

        [Header("Icons")]
        [SerializeField] private ControlIcon[] _controls = new ControlIcon[0];
        [SerializeField] private CompositeIcon[] _composites = new CompositeIcon[0];

        #endregion


        #region Private and Protected

        private Dictionary<string, Sprite> _controlMap;
        private Dictionary<string, Sprite> _compositeMap;

        #endregion
    }
}