using InputPrompts.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputPrompts.Runtime
{
    /// <summary>
    /// Brick 3 — UI view: persistent corner legend (Mad Max style).
    ///
    /// Listens to the device channel -> queries the resolver -> updates rows that
    /// are PRE-INSTANTIATED once. No runtime allocation when the device changes
    /// (rows are reused), bounded loops: Power of 10 friendly. No coroutine, no
    /// Find, DI 100% via [SerializeField].
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InputLegendView : MonoBehaviour
    {
        #region Unity API

        public void Awake()
        {
            BuildRows();
        }

        public void OnEnable()
        {
            if (_channel == null)
            {
                return;
            }

            _channel.OnChanged += HandleDeviceChanged;

            // Resync if the channel already raised before we subscribed.
            if (_channel.Current != null)
            {
                HandleDeviceChanged(_channel.Current);
            }
        }

        public void OnDisable()
        {
            if (_channel != null)
            {
                _channel.OnChanged -= HandleDeviceChanged;
            }
        }

        #endregion


        #region Tools and Utilities

        private void BuildRows()
        {
            if (_actionList == null || _rowPrefab == null || _container == null)
            {
                _rows = new InputLegendRow[0];
                return;
            }

            int count = _actionList.Count;
            _rows = new InputLegendRow[count];
            for (int i = 0; i < count; i++)
            {
                _rows[i] = Instantiate(_rowPrefab, _container);
                _rows[i].SetVisible(false);
            }
        }

        private void HandleDeviceChanged(InputDevice device)
        {
            _device = device;
            Refresh();
        }

        private void Refresh()
        {
            if (_rows == null || _resolver == null || _actionList == null || _device == null)
            {
                return;
            }

            for (int i = 0; i < _rows.Length; i++)
            {
                PromptActionList.Entry entry = _actionList.Get(i);
                InputAction action = entry.action != null ? entry.action.action : null;
                if (action == null)
                {
                    _rows[i].SetVisible(false);
                    continue;
                }

                if (_resolver.TryGetIcon(action, _device, out InputIconResult result))
                {
                    string label = string.IsNullOrEmpty(entry.label) ? action.name : entry.label;
                    _rows[i].Set(label, in result);
                    _rows[i].SetVisible(true);
                }
                else
                {
                    _rows[i].SetVisible(false);
                }
            }
        }

        #endregion


        #region Show In Inspector

        [Header("Dependencies")]
        [SerializeField] private InputDeviceChangedChannel _channel;
        [SerializeField] private InputIconResolver _resolver;
        [SerializeField] private PromptActionList _actionList;

        [Header("UI")]
        [SerializeField] private RectTransform _container;
        [SerializeField] private InputLegendRow _rowPrefab;

        #endregion


        #region Private and Protected

        private InputLegendRow[] _rows;
        private InputDevice _device;

        #endregion
    }
}