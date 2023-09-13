using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

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
        }

        private void Start()
        {
            Load();
        }

        private async void Load()
        {
            var scene = SceneManager.LoadSceneAsync(_currentLevelIndex);
            scene.allowSceneActivation = false;

            do
            {
                await Task.Delay(100);
            } while (scene.progress < 0.9f);

            scene.allowSceneActivation = true;
        }

        [ContextMenu("Delete Data")]
        private void DeleteData()
        {
            SaveLoadProgress.DeleteData(UniqSavingId.LevelProgression);
        }
    }
}

