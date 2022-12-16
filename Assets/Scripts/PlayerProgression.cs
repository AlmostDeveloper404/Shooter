using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main
{
    [System.Serializable]
    public struct LevelProgression
    {
        public int Level;
    }

    public class PlayerProgression : MonoBehaviour
    {
        private int _currentLevelIndex;

        private void Awake()
        {
            LevelProgression levelProgression = SaveLoadProgress.LoadData<LevelProgression>(UniqSavingId.LevelProgression);

            _currentLevelIndex = levelProgression.Equals(default(LevelProgression)) ? 1 : levelProgression.Level;
            SceneManager.LoadScene(_currentLevelIndex);
        }
    }
}

