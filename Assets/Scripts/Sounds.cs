using UnityEngine;
using Zenject;

namespace Main
{
    public class Sounds : MonoBehaviour
    {
        [SerializeField] private AudioSource[] _allAudio;

        [SerializeField] private AudioClip _backgroundMusic;
        [SerializeField] private AudioClip _winSound;
        [SerializeField] private AudioClip _lostSound;

        private CutSceneActivator _bossTriggerActivator;

        [Inject]
        private void Construct(CutSceneActivator bossTriggerActivator)
        {
            _bossTriggerActivator = bossTriggerActivator;
        }

        private void Awake()
        {
            _allAudio = new AudioSource[transform.childCount];
            for (int i = 0; i < _allAudio.Length; i++)
            {
                _allAudio[i] = transform.GetChild(i).GetComponent<AudioSource>();
            }
        }

        private void OnEnable()
        {
            GameManager.OnLevelCompleted += LevelCompleted;
            GameManager.OnGameOver += GameOver;
            _bossTriggerActivator.OnCutSceneEnded += StartBackgroundMusic;
        }

        private void OnDisable()
        {
            GameManager.OnLevelCompleted -= LevelCompleted;
            GameManager.OnGameOver -= GameOver;
            _bossTriggerActivator.OnCutSceneEnded -= StartBackgroundMusic;
        }


        public void PlaySound(AudioClip audioClip)
        {
            foreach (var clip in _allAudio)
            {
                if (clip.clip == audioClip) clip.Play();
            }
        }

        private void StartBackgroundMusic()
        {
            PlaySound(_backgroundMusic);
        }

        private void LevelCompleted()
        {
            StopSound(_backgroundMusic);
            PlaySound(_winSound);
        }

        private void GameOver()
        {
            StopSound(_backgroundMusic);
            PlaySound(_lostSound);
        }

        public void StopSound(AudioClip clip)
        {
            foreach (var item in _allAudio)
            {
                if (clip == item.clip) item.Stop();
            }
        }
    }
}


