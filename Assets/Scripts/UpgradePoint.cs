using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace Main
{
    public enum DropType { Upgrade, Clon, FireRate, HP }

    public class UpgradePoint : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private DropType _dropType;

        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TMP_Text _coinsCounter;

        [SerializeField] private int _coinsNeed;

        private int _coinsInvested;

        private CompositeDisposable _onEveryUpdateDis = new CompositeDisposable();
        private CompositeDisposable _onTriggerExitDis = new CompositeDisposable();

        [SerializeField] private Transform _target;

        [SerializeField] private Collider _exitCollider;

        private float _timer = 0;
        [SerializeField] private float _timeBetweenInvest = 0.2f;

        private PlayerInteractions _playerInteractions;
        private FloatingJoystick _floatingJoystick;

        [SerializeField] private GameObject _coin;

        [Inject]
        private void Construct(PlayerController controller, FloatingJoystick floatingJoystick)
        {
            _playerInteractions = controller.GetComponent<PlayerInteractions>();
            _floatingJoystick = floatingJoystick;

        }


        private void Start()
        {
            UpdateUpgradePoint();
        }

        public void Interact()
        {
            if (PlayerResources.MoneyAmount > 0)
            {
                Observable.EveryUpdate().Subscribe(_ => Invest()).AddTo(_onEveryUpdateDis);
                _exitCollider.OnTriggerExitAsObservable().Where(t => t.GetComponent<PlayerController>()).Subscribe(_ => StopInvesting()).AddTo(_onTriggerExitDis);
            }
        }

        private void Invest()
        {
            if (PlayerResources.MoneyAmount == 0 || _floatingJoystick.Horizontal != 0 || _floatingJoystick.Vertical != 0) return;

            _timer += Time.deltaTime;
            if (_timer > _timeBetweenInvest)
            {
                _timer = 0;
                _coinsInvested++;
                ShowAnimation();
                PlayerResources.RemoveMoney(1);
                UpdateUpgradePoint();
            }

        }

        private void UpdateUpgradePoint()
        {
            float fillAmount = (float)_coinsInvested / (float)_coinsNeed;
            _backgroundImage.fillAmount = fillAmount;
            _coinsCounter.text = $"{_coinsInvested}/{_coinsNeed}";

            if (_coinsInvested == _coinsNeed)
            {
                _timer = -0.5f;

                Upgrade();
            }
        }

        private void ShowAnimation()
        {
            Vector3 direction = _target.position - _playerInteractions.transform.position;
            Coin coin = CoinsSpawner.GetCoin(_coin.gameObject, _playerInteractions.transform.position + Vector3.up);
            coin.Launch(_target.position, direction.normalized, 85f);
            //coin.Launch(_target.position, direction.normalized, 45f);
        }

        private void StopInvesting()
        {
            _onEveryUpdateDis?.Clear();
            _onTriggerExitDis?.Clear();
        }

        private void Upgrade()
        {
            switch (_dropType)
            {
                case DropType.Upgrade:
                    break;
                case DropType.Clon:
                    break;
                case DropType.FireRate:
                    break;
                case DropType.HP:
                    break;
            }
        }
    }
}


