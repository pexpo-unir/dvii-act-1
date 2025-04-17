using UnityEngine;

namespace UI
{
    public class LookAtCameraUI : MonoBehaviour
    {
        private Camera _camera;

        private void Start() => _camera = Camera.main;

        private void LateUpdate()
        {
            if (_camera)
            {
                transform.LookAt(_camera.transform);
            }
        }
    }
}