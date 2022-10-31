using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace Main
{
    public class InteractionPointRooms : MonoBehaviour, IInteractable
    {
        [SerializeField] private Image _filledImage;
        private CompositeDisposable _everyUpdateDis = new CompositeDisposable();
        private CompositeDisposable _onTriggerExitDis = new CompositeDisposable();

        [SerializeField] private Room _targetRoom;

        private float _progress = 0;
        [SerializeField] private float _openingMultiplier = 0.5f;

        private bool _isOpened = false;

        private BoxCollider _boxCollider;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }

        public void Interact()
        {
            if (PlayerResources.KeysAmount != 0 && !_isOpened)
            {
                Observable.EveryUpdate().Subscribe(_ => Fill()).AddTo(_everyUpdateDis);
                _boxCollider.OnTriggerExitAsObservable().Where(t => t.GetComponent<PlayerController>()).Subscribe(_ => StopInteracting()).AddTo(_onTriggerExitDis);
            }

        }

        private void Fill()
        {
            _progress += Time.deltaTime * _openingMultiplier;


            if (_progress < 1)
            {
                _filledImage.fillAmount = _progress;
            }
            else
            {
                OpenRoom();
            }

        }

        private void OpenRoom()
        {
            PlayerResources.RemoveKey();
            enabled = false;
            _targetRoom.gameObject.SetActive(true);
            _filledImage.fillAmount = 1;
            _everyUpdateDis?.Clear();
            _onTriggerExitDis?.Clear();
        }

        private void StopInteracting()
        {
            Debug.Log("Stop");
            _onTriggerExitDis?.Clear();
            _everyUpdateDis?.Clear();
            _progress = 0;
            _filledImage.fillAmount = 0;
        }
    }
}

