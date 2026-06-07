using UnityEngine;
using InputPrompts.Runtime;

namespace InputPrompts.Samples
{
    /// <summary>
    /// EXAMPLE gameplay glue (NOT part of the generic feature). 3D version: shows an
    /// InteractionPrompt while a matching collider is inside this trigger, hides it on
    /// exit.
    ///
    /// Put this on the NPC/door with a trigger Collider (Box/Sphere) sized as the
    /// proximity zone, then assign the prompt. The player needs a Rigidbody and the
    /// matching tag so 3D trigger events fire.
    ///
    /// This lives in your GAME assembly, not in the package: proximity detection is
    /// gameplay logic, not an input-prompts concern.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class InteractionTrigger3D : MonoBehaviour
    {
        #region Unity API

        public void Reset()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (_prompt != null && other.CompareTag(_otherTag))
            {
                _prompt.Show();
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (_prompt != null && other.CompareTag(_otherTag))
            {
                _prompt.Hide();
            }
        }

        #endregion


        #region Show In Inspector

        [Header("Prompt")]
        [SerializeField] private InteractionPrompt _prompt;

        [Header("Filter")]
        [Tooltip("Only this tag triggers the prompt (usually the player).")]
        [SerializeField] private string _otherTag = "Player";

        #endregion
    }
}