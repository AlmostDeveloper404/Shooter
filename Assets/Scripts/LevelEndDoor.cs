using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;

namespace Main
{
    public class LevelEndDoor : MonoBehaviour
    {
        private Room _lastRoom;

        private Collider _endTrigger;

        private CompositeDisposable _onTriggerEnterDis = new CompositeDisposable();

        [SerializeField] private float _timeToLoadNext;

        private Animator _animator;
        [SerializeField] private ParticleSystem _endParticles;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _lastRoom = GetComponentInParent<Room>();
            _endTrigger = GetComponent<Collider>();
        }


        private void OnEnable()
        {
            _lastRoom.OnRoomCleared += EnableExit;
        }

        private void EnableExit()
        {
            _endParticles?.Play();
            _animator.SetTrigger(Animations.OpenDoor);
            _endTrigger.OnTriggerEnterAsObservable().Where(t => t.gameObject.GetComponent<PlayerController>()).Subscribe(_ => LevelCompleted()).AddTo(_onTriggerEnterDis);
        }

        private void LevelCompleted()
        {
            _onTriggerEnterDis?.Clear();
            GameManager.ChangeGameState(GameState.LevelCompleted);
            SaveLoadProgress.SaveData(new LevelProgression { Level = SceneManager.GetActiveScene().buildIndex + 1 }, UniqSavingId.LevelProgression);
            StartCoroutine(LoadNextLevel());
        }

        private void OnDisable()
        {
            _lastRoom.OnRoomCleared -= EnableExit;
        }

        private void OnDestroy()
        {
            _onTriggerEnterDis?.Clear();
        }

        private IEnumerator LoadNextLevel()
        {
            yield return Helpers.Helper.GetWait(_timeToLoadNext);
            GameManager.NextLevel();
        }
    }
}


