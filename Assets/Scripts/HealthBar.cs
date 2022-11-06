using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Main
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TMP_Text _text;

        private Camera _main;

        private void Awake()
        {
            _main = Camera.main;
        }

        private void Update()
        {
            transform.rotation = _main.transform.rotation;
        }

        public void UpdateUI(int maxHealth, int currentHealth)
        {
            _backgroundImage.fillAmount = (float)currentHealth / (float)maxHealth;

            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
            _text.text = $"{currentHealth}";
        }
    }
}

