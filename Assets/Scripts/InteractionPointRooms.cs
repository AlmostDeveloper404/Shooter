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

        private BoxCollider _boxCollider;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }

        public void Interact()
        {
            if (PlayerResources.KeysAmount != 0)
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
            _targetRoom.gameObject.SetActive(true);
            _filledImage.fillAmount = 1;
            _everyUpdateDis?.Clear();
            _onTriggerExitDis?.Clear();
            gameObject.SetActive(false);
        }

        private void StopInteracting()
        {
            _onTriggerExitDis?.Clear();
            _everyUpdateDis?.Clear();
            _progress = 0;
            _filledImage.fillAmount = 0;
        }
    }
}

