using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputPrompts.Runtime
{
    /// <summary>
    /// CONTRACT 1 — Device-changed event channel (ScriptableObject).
    ///
    /// Raised ONLY when the FAMILY of the last used device changes
    /// (e.g. Keyboard -> Xbox). The payload is the representative device of the
    /// new family; derive the family with InputDeviceFamilies.Classify.
    ///
    /// Frozen signature:  event Action&lt;InputDevice&gt; OnChanged
    /// plus a Current property so a late subscriber can re-sync.
    /// </summary>
    [CreateAssetMenu(
        fileName = "InputDeviceChangedChannel",
        menuName = "Input Prompts/Device Changed Channel")]
    public sealed class InputDeviceChangedChannel : ScriptableObject
    {
        #region Public API

        public event Action<InputDevice> OnChanged;

        // Last device raised (null until the first raise).
        public InputDevice Current { get; private set; }

        public void Raise(InputDevice device)
        {
            Current = device;
            OnChanged?.Invoke(device);
        }

        #endregion
    }
}