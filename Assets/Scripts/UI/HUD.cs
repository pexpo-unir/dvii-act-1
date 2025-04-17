using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        [Header("Player Resources")] [SerializeField]
        private ResourceBar healthBar;

        [SerializeField] private ResourceBar energyBar;

        [SerializeField] private ResourceBar defenseBar;

        [Header("Death UI Elements")] [SerializeField]
        private Image backdropDeath;

        [SerializeField] private TMP_Text loseText;

        [SerializeField] private Button restartButton;

        [SerializeField] private float backdropDeathAlphaEndValue = 0.99f;

        [SerializeField] private float backdropDeathEffectDuration = 5f;

        [SerializeField] private float backdropDeathElementInDuration = 0.5f;

        [SerializeField] private float backdropDeathElementInScale = 1f;

        private void Start()
        {
            loseText.transform.localScale = Vector3.zero;
            restartButton.transform.localScale = Vector3.zero;

            restartButton.onClick.AddListener(RestartLevel);
        }

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
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            var seq = DOTween.Sequence();
            seq.Append(backdropDeath.DOFade(backdropDeathAlphaEndValue, backdropDeathEffectDuration));
            seq.Append(loseText.transform.DOScale(backdropDeathElementInScale, backdropDeathElementInDuration)
                .SetEase(Ease.OutBack));
            seq.Append(restartButton.transform.DOScale(backdropDeathElementInScale, backdropDeathElementInDuration)
                .SetEase(Ease.OutBack));
        }

        private static void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}