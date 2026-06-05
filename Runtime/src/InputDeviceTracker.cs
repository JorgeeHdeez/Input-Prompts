using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace InputPrompts.Runtime
{
    /// <summary>
    /// Brick 1 — Last-used device detection.
    /// Raises the channel only when the FAMILY changes (not on every input).
    ///
    /// Three input sources (they can coexist; Report() dedupes):
    ///  1) Listen To Action Change (default): self-contained. Hooks
    ///     InputSystem.onActionChange and reads the device of whatever action just
    ///     fired. Works with ANY input setup, no PlayerInput, no host code needed.
    ///     This is the recommended path: drop the component in and you are done.
    ///  2) Listen To Input User: hooks InputUser.onChange. Only fires when you use a
    ///     PlayerInput with control-scheme auto-switching.
    ///  3) Report(device): manual. Call it from your own input layer if you prefer
    ///     to drive detection explicitly (e.g. from context.control.device).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InputDeviceTracker : MonoBehaviour
    {
        #region Unity API

        public void OnEnable()
        {
            if (_listenToActionChange)
            {
                InputSystem.onActionChange += HandleActionChange;
            }

            if (_listenToInputUser)
            {
                InputUser.onChange += HandleUserChange;
            }

            RaiseInitial();
        }

        public void OnDisable()
        {
            if (_listenToActionChange)
            {
                InputSystem.onActionChange -= HandleActionChange;
            }

            if (_listenToInputUser)
            {
                InputUser.onChange -= HandleUserChange;
            }
        }

        #endregion


        #region Public API

        /// <summary>
        /// Manual entry point. Raises the channel only when the family changes.
        /// </summary>
        public void Report(InputDevice device)
        {
            if (device == null || _channel == null)
            {
                return;
            }

            DeviceFamily family = InputDeviceFamilies.Classify(device);
            if (family == DeviceFamily.Unknown || family == _currentFamily)
            {
                return;
            }

            _currentFamily = family;
            _channel.Raise(device);
        }

        #endregion


        #region Tools and Utilities

        private void RaiseInitial()
        {
            // Sensible desktop default: keyboard if present, otherwise gamepad.
            InputDevice initial = Keyboard.current != null
                ? (InputDevice)Keyboard.current
                : Gamepad.current;

            if (initial != null)
            {
                Report(initial);
            }
        }

        // Self-contained detection: fired whenever an enabled action performs.
        // Cheap: Report() early-outs unless the family actually changes.
        private void HandleActionChange(object actionObject, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed)
            {
                return;
            }

            if (actionObject is InputAction action && action.activeControl != null)
            {
                // Ignore analog noise / stick drift: only count real actuation.
                if (action.activeControl.IsActuated(_actuationThreshold))
                {
                    Report(action.activeControl.device);
                }
            }
        }

        private void HandleUserChange(
            InputUser user, InputUserChange change, InputDevice device)
        {
            bool relevant =
                change == InputUserChange.ControlSchemeChanged ||
                change == InputUserChange.DevicePaired ||
                change == InputUserChange.DeviceRegained;

            if (relevant && device != null)
            {
                Report(device);
            }
        }

        #endregion


        #region Show In Inspector

        [Header("Channel")]
        [SerializeField] private InputDeviceChangedChannel _channel;

        [Header("Detection sources")]
        [Tooltip("Self-contained: reads the device of whatever action just fired. " +
                 "Recommended; works without PlayerInput.")]
        [SerializeField] private bool _listenToActionChange = true;

        [Tooltip("Hook InputUser.onChange (only fires with a PlayerInput auto-switch).")]
        [SerializeField] private bool _listenToInputUser = false;
        
        [Tooltip("Minimum actuation to count as a real input (filters stick drift).")]
        [SerializeField] private float _actuationThreshold = 0.5f;

        #endregion


        #region Private and Protected

        private DeviceFamily _currentFamily = DeviceFamily.Unknown;

        #endregion
    }
}