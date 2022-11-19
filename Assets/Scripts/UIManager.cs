using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Main
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;

        [SerializeField] private TMP_Text _keysText;
        [SerializeField] private TMP_Text _moneyText;

        [SerializeField] private TMP_Text _endText;

        [SerializeField] private GameObject _blockPanal;

        private CutSceneActivator _bossTriggerActivator;

        [Inject]
        private void Construct(CutSceneActivator bossTriggerActivator)
        {
            _bossTriggerActivator = bossTriggerActivator;
        }

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

            _bossTriggerActivator.OnCutSceneEnded += StopCutScene;
            _bossTriggerActivator.OnBossFight += StartCutScene;
            _restartButton.onClick.AddListener(() => GameManager.Restart());
        }

        private void OnDisable()
        {
            PlayerResources.OnMoneyAmountChanged -= UpdateMoney;
            PlayerResources.OnKeysAmountChanged -= UpdateKeys;
            GameManager.OnGameOver -= GameOver;
            GameManager.OnLevelCompleted -= LevelCompleted;

            _bossTriggerActivator.OnCutSceneEnded -= StopCutScene;
            _bossTriggerActivator.OnBossFight -= StartCutScene;
            _restartButton.onClick.RemoveAllListeners();
        }
        private void LevelCompleted()
        {
            _endText.color = Color.green;
            _endText.text = "Victory!";
        }

        private void GameOver()
        {
            _endText.color = Color.red;
            _endText.text = "FAIL";
        }

        private void StartCutScene()
        {
            _blockPanal.SetActive(true);
        }

        private void StopCutScene()
        {
            _blockPanal.SetActive(false);
        }

        private void UpdateKeys(int amount)
        {
            _keysText.text = amount.ToString();
        }

        private void UpdateMoney(int amount)
        {
            _moneyText.text = amount.ToString();
        }

    }
}



