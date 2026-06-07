using UnityEngine;
using UnityEngine.InputSystem;

namespace InputPrompts.Runtime
{
    /// <summary>
    /// World-space contextual prompt (e.g. "A : Grab" above an NPC or a door).
    ///
    /// Reuses the SAME resolver and the SAME device channel as the legend, and an
    /// InputLegendRow for the visual. Re-resolves live when the device family changes
    /// (keyboard &lt;-&gt; gamepad), even while shown.
    ///
    /// Drive it from your interaction logic: call Show() when the player can interact
    /// and Hide() when they leave. Optionally Configure(...) to reuse one prompt for
    /// different actions/labels.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InteractionPrompt : MonoBehaviour
    {
        #region Unity API

        public void OnEnable()
        {
            if (_channel != null)
            {
                _channel.OnChanged += HandleDeviceChanged;
            }

            SetShown(_shownOnEnable);
        }

        public void OnDisable()
        {
            if (_channel != null)
            {
                _channel.OnChanged -= HandleDeviceChanged;
            }
        }

        #endregion


        #region Public API

        /// <summary>Optionally retarget this prompt before (or while) showing it.</summary>
        public void Configure(InputActionReference action, string label)
        {
            _action = action;
            _label = label;

            if (_shown)
            {
                Refresh();
            }
        }

        public void Show()
        {
            SetShown(true);
        }

        public void Hide()
        {
            SetShown(false);
        }

        #endregion


        #region Tools and Utilities

        private void SetShown(bool shown)
        {
            _shown = shown;

            if (shown)
            {
                Refresh();
            }
            else if (_rowNPC != null)
            {
                _rowNPC.SetVisible(false);
            }
        }

        private void HandleDeviceChanged(InputDevice device)
        {
            if (_shown)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            if (_rowNPC == null || _resolver == null || _channel == null)
            {
                return;
            }

            InputAction action = _action != null ? _action.action : null;
            InputDevice device = _channel.Current;
            if (action == null || device == null)
            {
                _rowNPC.SetVisible(false);
                return;
            }

            if (_resolver.TryGetIcon(action, device, out InputIconResult result))
            {
                _rowNPC.Set(_label, in result);
                _rowNPC.SetVisible(true);
            }
            else
            {
                _rowNPC.SetVisible(false);
            }
        }

        #endregion


        #region Show In Inspector

        [Header("Dependencies")]
        [SerializeField] private InputDeviceChangedChannel _channel;
        [SerializeField] private InputIconResolver _resolver;

        [Header("Prompt")]
        [SerializeField] private InputActionReference _action;
        [SerializeField] private string _label = "Interact";
        [SerializeField] private InputLegendRow _rowNPC;

        [Header("Behaviour")]
        [Tooltip("Start visible (handy to preview in the editor). Usually false.")]
        [SerializeField] private bool _shownOnEnable = false;

        #endregion


        #region Private and Protected

        private bool _shown;

        #endregion
    }
}