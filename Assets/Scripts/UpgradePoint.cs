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


        [Header("UI Reference")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TMP_Text _coinsCounter;
        [SerializeField] private Image _dropTypeImage;
        [SerializeField] private TMP_Text _upgradeDropName;
        [SerializeField] private Sprite _fireRate;
        [SerializeField] private Sprite _health;
        [SerializeField] private Sprite _clon;
        [SerializeField] private Sprite _damage;
        [SerializeField] private Image _frontImage;

        [SerializeField] private int _coinsNeed;

        private int _coinsInvested;

        private CompositeDisposable _onEveryUpdateDis = new CompositeDisposable();
        private CompositeDisposable _onTriggerExitDis = new CompositeDisposable();


        public Vector3 ClonSpawnPosition { get { return _target.position; } }

        [SerializeField] private Collider _exitCollider;
        [SerializeField] private float _timeBetweenInvest = 0.2f;
        [SerializeField] private float _angle = 45f;
        [SerializeField] private Transform _target;


        private float _timer = 0;

        private PlayerUpgrade _playerUpgrade;
        private FloatingJoystick _floatingJoystick;
        private CoinsSpawner _coinSpawner;



        [Inject]
        private void Construct(PlayerController controller, FloatingJoystick floatingJoystick,CoinsSpawner coinsSpawner)
        {
            _playerUpgrade = controller.GetComponent<PlayerUpgrade>();
            _floatingJoystick = floatingJoystick;
            _coinSpawner = coinsSpawner;
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
                    _dropTypeImage.sprite = _damage;
                    _frontImage.color = Color.red;
                    break;
                case DropType.Clon:
                    _dropTypeImage.sprite = _clon;
                    _frontImage.color = Color.cyan;
                    break;
                case DropType.FireRate:
                    
                    _dropTypeImage.sprite = _fireRate;
                    _frontImage.color = Color.blue;
                    break;
                case DropType.HP:
                    _dropTypeImage.sprite = _health;
                    _frontImage.color = Color.green;
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

            int coinsLeft = _coinsNeed - _coinsInvested;

            _coinsCounter.text = $"{coinsLeft}";

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
            CoinGrx coin = _coinSpawner.GetCoinForGrx(_playerUpgrade.transform.position + Vector3.up);
            coin.Launch(_target.position, direction.normalized, _angle);
        }

        private void StopInvesting()
        {
            _onEveryUpdateDis?.Clear();
            _onTriggerExitDis?.Clear();
        }

        private void Upgrade()
        {
            UpdateUpgradePoint();
            _playerUpgrade.UpgradeCharacter(this, _dropType);
        }
    }
}


