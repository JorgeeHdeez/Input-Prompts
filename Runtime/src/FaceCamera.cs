using UnityEngine;

namespace InputPrompts.Runtime
{
    /// <summary>
    /// Billboards this transform so it always faces a camera. Use it on a world-space
    /// prompt Canvas in 3D, so it stays readable when the camera moves around it.
    ///
    /// The camera is injected (no hidden Camera.main lookup). If left empty, it resolves
    /// Camera.main once on enable, as a convenience fallback.
    ///
    /// Not needed in 2D or with a fixed camera.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FaceCamera : MonoBehaviour
    {
        #region Unity API

        public void OnEnable()
        {
            if (_camera == null && Camera.main != null)
            {
                _camera = Camera.main.transform;
            }
        }

        public void LateUpdate()
        {
            if (_camera == null)
            {
                return;
            }

            transform.forward = _camera.forward;
        }

        #endregion


        #region Show In Inspector

        [Tooltip("Camera to face. Leave empty to use Camera.main on enable.")]
        [SerializeField] private Transform _camera;

        #endregion
    }
}