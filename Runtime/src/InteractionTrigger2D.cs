using UnityEngine;
using InputPrompts.Runtime;

namespace InputPrompts.Samples
{
    /// <summary>
    /// EXAMPLE gameplay glue (NOT part of the generic feature). Shows an
    /// InteractionPrompt while a matching collider is inside this trigger, and hides
    /// it on exit.
    ///
    /// Put this on the NPC/door with a trigger Collider2D sized as the proximity zone,
    /// then assign the prompt. The player needs a Rigidbody2D and the matching tag so
    /// 2D trigger events fire.
    ///
    /// This lives in your GAME assembly, not in the package: proximity detection is
    /// gameplay logic, not an input-prompts concern.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class InteractionTrigger2D : MonoBehaviour
    {
        #region Unity API

        public void Reset()
        {
            var collider2d = GetComponent<Collider2D>();
            collider2d.isTrigger = true;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (_prompt != null && other.CompareTag(_otherTag))
            {
                _prompt.Show();
            }
        }

        public void OnTriggerExit2D(Collider2D other)
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