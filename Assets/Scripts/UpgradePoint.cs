using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace Main
{

    public class UpgradePoint : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private DropType _dropType;

        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TMP_Text _coinsCounter;
        [SerializeField] private Image _dropTypeImage;
        [SerializeField] private TMP_Text _upgradeDropName;

        [SerializeField] private int _coinsNeed;

        private int _coinsInvested;

        private CompositeDisposable _onEveryUpdateDis = new CompositeDisposable();
        private CompositeDisposable _onTriggerExitDis = new CompositeDisposable();

        [SerializeField] private Transform _target;

        public Vector3 ClonSpawnPosition { get { return _target.position; } }

        [SerializeField] private Collider _exitCollider;


        private float _timer = 0;
        [SerializeField] private float _timeBetweenInvest = 0.2f;

        private PlayerUpgrade _playerUpgrade;
        private FloatingJoystick _floatingJoystick;

        [SerializeField] private GameObject _coin;

        [SerializeField] private float _angle = 45f;

        [Inject]
        private void Construct(PlayerController controller, FloatingJoystick floatingJoystick)
        {
            _playerUpgrade = controller.GetComponent<PlayerUpgrade>();
            _floatingJoystick = floatingJoystick;
        }


        private void Start()
        {
            UpdateUpgradePoint();
            SetupPoint();
        }

        public void Interact()
        {
            if (PlayerResources.MoneyAmount > 0)
            {
                Observable.EveryUpdate().Subscribe(_ => Invest()).AddTo(_onEveryUpdateDis);
                _exitCollider.OnTriggerExitAsObservable().Where(t => t.GetComponent<PlayerController>()).Subscribe(_ => StopInvesting()).AddTo(_onTriggerExitDis);
            }
        }

        private void SetupPoint()
        {
            switch (_dropType)
            {
                case DropType.Damage:
                    _upgradeDropName.text = $"Damage";
                    break;
                case DropType.Clon:
                    _upgradeDropName.text = $"Clon";
                    break;
                case DropType.FireRate:
                    _upgradeDropName.text = $"FireRate";
                    break;
                case DropType.HP:
                    _upgradeDropName.text = $"Health";
                    break;
                default:
                    break;
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
                _backgroundImage.fillAmount = 0;
                _coinsInvested = 0;
                Upgrade();
            }
        }

        private void ShowAnimation()
        {
            Vector3 direction = _target.position - _playerUpgrade.transform.position;
            CoinGrx coin = CoinsSpawner.GetCoinForGrx(_coin.gameObject, _playerUpgrade.transform.position + Vector3.up);
            coin.Launch(_target.position, direction.normalized, _angle);
        }

        private void StopInvesting()
        {
            _onEveryUpdateDis?.Clear();
            _onTriggerExitDis?.Clear();
        }

        private void Upgrade()
        {
            _playerUpgrade.UpgradeCharacter(this, _dropType);
        }
    }
}


