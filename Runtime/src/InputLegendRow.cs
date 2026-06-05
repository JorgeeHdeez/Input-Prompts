using InputPrompts.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InputPrompts.Runtime
{
    /// <summary>
    /// A single legend row: label + icon.
    /// If no sprite is resolved, the fallback text (e.g. "Space") is shown instead
    /// of the icon, so a row is never left empty.
    ///
    /// Note: uses TMP_Text (TextMeshPro, standard in Unity 6). Swap for
    /// UnityEngine.UI.Text if you do not want the TMP dependency.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InputLegendRow : MonoBehaviour
    {
        #region Public API

        public void Set(string label, in InputIconResult result)
        {
            if (_label != null)
            {
                _label.text = label;
            }

            bool hasSprite = result.HasSprite;

            if (_icon != null)
            {
                _icon.enabled = hasSprite;
                if (hasSprite)
                {
                    _icon.sprite = result.Sprite;
                }
            }

            if (_fallbackText != null)
            {
                _fallbackText.enabled = !hasSprite;
                _fallbackText.text = hasSprite ? string.Empty : result.Fallback;
            }
        }

        public void SetVisible(bool visible)
        {
            if (gameObject.activeSelf != visible)
            {
                gameObject.SetActive(visible);
            }
        }

        #endregion


        #region Show In Inspector

        [Header("References")]
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _fallbackText;

        #endregion
    }
}