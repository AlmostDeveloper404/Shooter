using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

namespace Main
{
    public class LevelEndDoor : MonoBehaviour
    {
        [SerializeField] private Room _lastRoom;

        [SerializeField] private Collider _endTrigger;

        private CompositeDisposable _onTriggerEnterDis = new CompositeDisposable();

        [SerializeField] private float _timeToLoadNext;


        private void OnEnable()
        {
            _lastRoom.OnRoomCleared += EnableExit;
        }

        private void EnableExit()
        {
            _endTrigger.OnTriggerEnterAsObservable().Where(t => t.gameObject.GetComponent<PlayerController>()).Subscribe(_ => LevelCompleted()).AddTo(_onTriggerEnterDis);
        }

        private void LevelCompleted()
        {
            _onTriggerEnterDis?.Clear();
            GameManager.ChangeGameState(GameState.LevelCompleted);
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


