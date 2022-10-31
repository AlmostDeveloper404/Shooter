using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Main
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;

        [SerializeField] private TMP_Text _keysText;
        [SerializeField] private TMP_Text _moneyText;

        private void Start()
        {
            UpdateKeys(0);
            UpdateMoney(0);
        }

        private void OnEnable()
        {
            PlayerResources.OnKeysAmountChanged += UpdateKeys;
            PlayerResources.OnMoneyAmountChanged += UpdateMoney;
            GameManager.OnGameOver += GameOver;
            GameManager.OnLevelCompleted += LevelCompleted;

            _restartButton.onClick.AddListener(() => GameManager.Restart());
        }

        private void LevelCompleted()
        {
            
        }

        private void GameOver()
        {
            
        }

        private void UpdateKeys(int amount)
        {
            _keysText.text = amount.ToString();
        }

        private void UpdateMoney(int amount)
        {
            _moneyText.text = amount.ToString();
        }

        private void OnDisable()
        {
            PlayerResources.OnMoneyAmountChanged -= UpdateMoney;
            PlayerResources.OnKeysAmountChanged -= UpdateKeys;
            GameManager.OnGameOver -= GameOver;
            GameManager.OnLevelCompleted -= LevelCompleted;

            _restartButton.onClick.RemoveAllListeners();
        }
    }
}



