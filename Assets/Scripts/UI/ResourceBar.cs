using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ResourceBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;

        public void SetPercentValue(float percent)
        {
            fillImage.fillAmount = percent;
        }
    }
}
