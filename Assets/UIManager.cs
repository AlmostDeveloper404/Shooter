using UnityEngine;
using TMPro;

namespace Main 
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _endText;

        private void OnEnable()
        {
            GameManager.OnGameOver += GameOver;
            GameManager.OnLevelCompleted += LevelCompleted;
        }

        private void LevelCompleted()
        {
            _endText.text = "Completed!";
        }

        private void GameOver()
        {
            _endText.text = "Failed!";
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= GameOver;
            GameManager.OnLevelCompleted -= LevelCompleted;
        }
    }
}



