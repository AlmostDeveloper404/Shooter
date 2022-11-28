using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using TMPro;
using Zenject;

namespace Main
{
    public class InteractionPointRooms : MonoBehaviour, IInteractable
    {
        [SerializeField] private Image _filledImage;
        [SerializeField] private Image _icon;
        private CompositeDisposable _everyUpdateDis = new CompositeDisposable();
        private CompositeDisposable _onTriggerExitDis = new CompositeDisposable();
        private CompositeDisposable _openDoorDis = new CompositeDisposable();
        private CompositeDisposable _openExtraDis = new CompositeDisposable();

        [SerializeField] private Room _targetRoom;

        private float _progress = 0;
        [SerializeField] private float _openingMultiplier = 0.5f;

        private BoxCollider _boxCollider;

        [SerializeField] private int _keysNeeded;

        [SerializeField] private Collider _door;
        [SerializeField] private Collider _extraDoor;


        [SerializeField] private float _lerpSpeed;

        private float _totalAngleToRotate;
        private float _totalAngleToRotateExtra;

        private float _y;
        private float _yExtra;

        [SerializeField] private float _targetY;
        [SerializeField] private float _extraDoorYTarget;

        [SerializeField] private bool _isBossDoor;

        private Sounds _sounds;
        [SerializeField] private AudioClip _doorOpening;


        [Inject]
        private void Construct(Sounds sounds)
        {
            _sounds = sounds;
        }

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }

        private void Start()
        {
            Setup();
        }

        private void Setup()
        {
            _icon.enabled = _keysNeeded == 0 ? false : true;

            if (_isBossDoor)
            {
                _yExtra = _extraDoor.transform.eulerAngles.y;
                _totalAngleToRotateExtra = Mathf.Abs(_extraDoor.transform.eulerAngles.y - _extraDoorYTarget);
            }

            _y = _door.transform.eulerAngles.y;
            _totalAngleToRotate = Mathf.Abs(_door.transform.eulerAngles.y - _targetY);
        }

        public void Interact()
        {
            if (PlayerResources.KeysAmount >= _keysNeeded)
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
                Observable.EveryUpdate().Subscribe(_ => OpenDoor()).AddTo(_openDoorDis);

                if (_isBossDoor) Observable.EveryUpdate().Subscribe(_ => OpenDoor(_extraDoor)).AddTo(_openExtraDis);
                OpenRoom();
            }

        }

        private void OpenRoom()
        {
            _sounds.PlaySound(_doorOpening);
            PlayerResources.RemoveKey(_keysNeeded);
            _targetRoom.gameObject.SetActive(true);
            _filledImage.fillAmount = 1;
            _everyUpdateDis?.Clear();
            _onTriggerExitDis?.Clear();
            gameObject.SetActive(false);
        }

        private void OpenDoor()
        {
            _door.isTrigger = true;
            Vector3 _doorEuler = _door.transform.eulerAngles;

            _y = Mathf.Lerp(_y, _targetY, _lerpSpeed * Time.deltaTime);
            _doorEuler.y = _y;
            _door.transform.eulerAngles = _doorEuler;


            float angleToRotate = Mathf.Abs(_y - _targetY);
            float currentAngle = _totalAngleToRotate - angleToRotate;


            float progress = currentAngle / _totalAngleToRotate;

            if (Mathf.Abs(progress) > 0.99f)
            {
                _door.transform.eulerAngles = new Vector3(_door.transform.eulerAngles.x, _targetY, _door.transform.eulerAngles.z);
                _door.isTrigger = false;
                _openDoorDis?.Clear();
            }
        }
        private void OpenDoor(Collider extraDoor)
        {
            extraDoor.isTrigger = true;
            Vector3 _doorEuler = extraDoor.transform.eulerAngles;

            _yExtra = Mathf.Lerp(_yExtra, _extraDoorYTarget, _lerpSpeed * Time.deltaTime);
            _doorEuler.y = _yExtra;
            extraDoor.transform.eulerAngles = _doorEuler;


            float angleToRotate = Mathf.Abs(_yExtra - _extraDoorYTarget);
            float currentAngle = _totalAngleToRotateExtra - angleToRotate;


            float progress = currentAngle / _totalAngleToRotateExtra;

            if (Mathf.Abs(progress) > 0.99f)
            {
                extraDoor.transform.eulerAngles = new Vector3(extraDoor.transform.eulerAngles.x, _extraDoorYTarget, extraDoor.transform.eulerAngles.z);
                extraDoor.isTrigger = false;
                _openExtraDis?.Clear();

            }
        }

        private void StopInteracting()
        {
            _onTriggerExitDis?.Clear();
            _everyUpdateDis?.Clear();
            _progress = 0;
            _filledImage.fillAmount = 0;
        }

        private void OnDestroy()
        {
            _everyUpdateDis?.Clear();
            _onTriggerExitDis?.Clear();
            _openDoorDis?.Clear();
            _openExtraDis?.Clear();
        }
    }
}

