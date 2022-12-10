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
        [SerializeField] private Image _dropTypeImage;
        [SerializeField] private Image _coinImage;

        [SerializeField] private TMP_Text _coinsCounter;
        [SerializeField] private TMP_Text _upgradeDropName;

        [SerializeField] private Sprite _fireRate;
        [SerializeField] private Sprite _health;
        [SerializeField] private Sprite _clon;
        [SerializeField] private Sprite _damage;
        [SerializeField] private Sprite _radius;
        [SerializeField] private Sprite _speed;

        [SerializeField] private Image _frontImage;

        private int _coinsInvested;

        private CompositeDisposable _onEveryUpdateDis = new CompositeDisposable();
        private CompositeDisposable _onTriggerExitDis = new CompositeDisposable();


        public Vector3 ClonSpawnPosition { get { return _target.position; } }

        [Header("For Investing")]
        [SerializeField] private int _coinsNeed = 10;
        [SerializeField] private Collider _exitCollider;
        [SerializeField] private float _timeBetweenInvest = 0.2f;
        [SerializeField] private float _angle = 45f;
        [SerializeField] private Transform _target;
        [SerializeField] private ParticleSystem _upgradePart;
        [SerializeField] private ParticleSystem _gettingItemPart;
        [SerializeField] private AudioClip _investingSound;


        private float _timer = 0;

        private PlayerUpgrade _playerUpgrade;
        private FloatingJoystick _floatingJoystick;
        private CoinsSpawner _coinSpawner;
        private Sounds _sounds;

        private int _upgradesAmount = 0;
        [SerializeField] private int _maxUpgrades = 3;


        [Inject]
        private void Construct(PlayerController controller, FloatingJoystick floatingJoystick, CoinsSpawner coinsSpawner, Sounds sounds)
        {
            _playerUpgrade = controller.GetComponent<PlayerUpgrade>();
            _floatingJoystick = floatingJoystick;
            _coinSpawner = coinsSpawner;
            _sounds = sounds;
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
                case DropType.AttackRadius:
                    _dropTypeImage.sprite = _radius;
                    _frontImage.color = Color.gray;
                    break;
                case DropType.Speed:
                    _dropTypeImage.sprite = _speed;
                    _frontImage.color = Color.magenta;
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
                _sounds.PlaySound(_investingSound);
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
            switch (_dropType)
            {
                case DropType.Damage:
                    _upgradePart.transform.position = _playerUpgrade.transform.position + Vector3.up * 2f;
                    _upgradePart.Play();
                    break;
                case DropType.Clon:
                    _gettingItemPart.Play();
                    break;
                case DropType.FireRate:
                    _upgradePart.transform.position = _playerUpgrade.transform.position + Vector3.up * 2f;
                    _upgradePart.Play();
                    break;
                case DropType.HP:
                    _gettingItemPart.Play();
                    break;
                case DropType.Speed:

                    break;
                case DropType.AttackRadius:

                    break;
                default:
                    break;
            }
            _playerUpgrade.UpgradeCharacter(this, _dropType);
            _upgradesAmount++;


            if (_upgradesAmount == _maxUpgrades)
            {
                DisablePoint();
            }

        }



        public void DisablePoint()
        {
            _onEveryUpdateDis?.Clear();
            _onTriggerExitDis?.Clear();

            _coinsCounter.alignment = TextAlignmentOptions.Center;
            _coinsCounter.text = $"MAX";

            _coinImage.enabled = false;
            _exitCollider.enabled = false;
        }

        private void OnDestroy()
        {
            _onEveryUpdateDis?.Clear();
            _onTriggerExitDis?.Clear();
        }
    }
}


