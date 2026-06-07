using UnityEngine;
using UnityEngine.InputSystem;

namespace InputPrompts.Runtime
{
    /// <summary>
    /// CONTRACT 2 — Icon resolver (ScriptableObject, shared service).
    ///
    /// Frozen signature:
    ///     bool TryGetIcon(InputAction action, InputDevice device, out InputIconResult result)
    ///
    ///  - true:  a displayable binding was found for this device. result.Sprite
    ///           may be null (missing sprite) -> use result.Fallback (text).
    ///  - false: no binding matches the device family.
    ///
    /// COMPOSITES = Option A: 1 composite binding -> 1 glyph (WASD cluster / stick),
    /// never 4 sprites. Resolution reads the device via the composite's first part.
    ///
    /// CONTEXTUAL later: a future "contextual prompt" (Zelda-style) will call this
    /// EXACT same TryGetIcon. Only the front-end (trigger) will differ.
    /// </summary>
    [CreateAssetMenu(
        fileName = "InputIconResolver",
        menuName = "Input Prompts/Icon Resolver")]
    public sealed class InputIconResolver : ScriptableObject
    {
        #region Public API

        public bool TryGetIcon(InputAction action, InputDevice device, out InputIconResult result)
        {
            result = default;
            if (action == null || device == null) return false;

            DeviceFamily family = InputDeviceFamilies.Classify(device);
            InputIconSet set = FindSet(family);

            int bindingIndex = SelectBindingIndex(action, family);
            if (bindingIndex < 0) return false;

            InputBinding binding = action.bindings[bindingIndex];
            string fallback = action.GetBindingDisplayString(bindingIndex);

            Sprite sprite = null;
            if (binding.isComposite)
            {
                if (set != null) set.TryGetComposite(binding.path, out sprite);
            }
            else
            {
                string token = ExtractControlToken(binding.effectivePath);
                if (set != null && token.Length > 0) set.TryGetControl(token, out sprite);
            }

            result = new InputIconResult(sprite, fallback);
            return true;
        }

        #endregion


        #region Tools and Utilities

        private InputIconSet FindSet(DeviceFamily family)
        {
            InputIconSet exact = FindByFamily(family);
            if (exact != null) return exact;

            if (InputDeviceFamilies.IsGamepad(family)) return FindByFamily(DeviceFamily.GenericGamepad);

            return null;
        }

        private InputIconSet FindByFamily(DeviceFamily family)
        {
            for (int i = 0; i < _iconSets.Length; i++)
            {
                if (_iconSets[i] != null && _iconSets[i].Family == family) return _iconSets[i];
            }

            return null;
        }

        private static int SelectBindingIndex(InputAction action, DeviceFamily family)
        {
            bool wantGamepad = InputDeviceFamilies.IsGamepad(family);
            var bindings = action.bindings;

            for (int i = 0; i < bindings.Count; i++)
            {
                InputBinding b = bindings[i];

                if (b.isPartOfComposite) continue;

                string path = b.isComposite ? FirstPartPath(bindings, i) : b.effectivePath;
                if (string.IsNullOrEmpty(path)) continue;

                if (BindingMatchesFamily(path, wantGamepad)) return i;
            }

            return -1;
        }

        private static string FirstPartPath(
            UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputBinding> bindings,
            int compositeIndex)
        {
            for (int i = compositeIndex + 1; i < bindings.Count; i++)
            {
                if (!bindings[i].isPartOfComposite) break;

                if (!string.IsNullOrEmpty(bindings[i].effectivePath)) return bindings[i].effectivePath;
            }

            return string.Empty;
        }
       
        private static bool BindingMatchesFamily(string effectivePath, bool wantGamepad)
        {
            string deviceLayout = InputControlPath.TryGetDeviceLayout(effectivePath);
            if (string.IsNullOrEmpty(deviceLayout)) return false;

            bool isKeyboardMouse = deviceLayout == "Keyboard" || deviceLayout == "Mouse";
            return wantGamepad ? !isKeyboardMouse : isKeyboardMouse;
        }

        private static string ExtractControlToken(string effectivePath)
        {
            if (string.IsNullOrEmpty(effectivePath)) return string.Empty;

            int close = effectivePath.IndexOf('>');
            int slash = effectivePath.IndexOf('/', close < 0 ? 0 : close);
            if (slash < 0 || slash + 1 >= effectivePath.Length) return string.Empty;

            return effectivePath.Substring(slash + 1);
        }

        #endregion


        #region Show In Inspector

        [Header("Icon Sets")]
        [SerializeField] private InputIconSet[] _iconSets = new InputIconSet[0];

        #endregion
    }
}