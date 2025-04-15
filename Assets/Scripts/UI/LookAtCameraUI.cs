using UnityEngine;

namespace UI
{
    public class LookAtCameraUI : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
