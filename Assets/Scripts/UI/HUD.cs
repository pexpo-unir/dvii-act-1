using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private ResourceBar healthBar;
        [SerializeField] private ResourceBar energyBar;
        [SerializeField] private ResourceBar defenseBar;

        [SerializeField] private Image backdropDeath;

        public void UpdateHealth(float value)
        {
            healthBar.SetPercentValue(value);
        }

        public void UpdateEnergy(float value)
        {
            energyBar.SetPercentValue(value);
        }

        public void UpdateDefense(float value)
        {
            defenseBar.SetPercentValue(value);
        }

        public void Death()
        {
            StartCoroutine(DeathCoroutine());
        }

        private IEnumerator DeathCoroutine()
        {
            var color = backdropDeath.color;

            while (color.a < 1)
            {
                color = new Color(color.r, color.g, color.b, color.a + 0.05f);
                backdropDeath.color = color;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}