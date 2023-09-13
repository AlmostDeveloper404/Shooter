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
        [SerializeField] private TMP_Text _titleText;

        [SerializeField] private Sprite _fireRate;
        [SerializeField] private Sprite _health;
        [SerializeField] private Sprite _clon;
        [SerializeField] private Sprite _damage;
        [SerializeField] private Sprite _radius;
        [SerializeField] private Sprite _speed;

        [SerializeField] private Image _frontImage;

        private int _coinsInvested;

        [SerializeField] private float _timeToNextUpgrade;

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
        [SerializeField] private ParticleSystem _pointGlow;
        [SerializeField] private AudioClip _investingSound;

        private Animation _animation;


        private float _timer = 0;

        private PlayerUpgrade _playerUpgrade;
        private FloatingJoystick _floatingJoystick;
        private CoinsSpawner _coinSpawner;
        private Sounds _sounds;
        private PlayerResources _playerResources;

        private int _upgradesAmount = 0;
        private int _maxUpgrades = 99;
        [SerializeField] private int _maxClonUpgrades;


        [Inject]
        private void Construct(PlayerController controller, FloatingJoystick floatingJoystick, CoinsSpawner coinsSpawner, Sounds sounds, PlayerResources playerResources)
        {
            _playerUpgrade = controller.GetComponent<PlayerUpgrade>();
            _floatingJoystick = floatingJoystick;
            _coinSpawner = coinsSpawner;
            _sounds = sounds;
            _playerResources = playerResources;
        }

        private void Awake()
        {
            _animation = GetComponent<Animation>();
        }


        private void Start()
        {
            UpdateUpgradePoint();
            SetupPoint();
        }

        public void Interact()
        {
            if (_playerResources.MoneyAmount > 0)
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
                    _dropTypeImage.color= Color.red;
                    _backgroundImage.color = Color.red;
                    _titleText.text = "DAMAGE";
                    break;
                case DropType.Clon:
                    _dropTypeImage.sprite = _clon;
                    _dropTypeImage.color = Color.cyan;
                    _backgroundImage.color = Color.cyan;

                    break;
                case DropType.FireRate:
                    _dropTypeImage.sprite = _fireRate;
                    _dropTypeImage.color = Color.blue;
                    _backgroundImage.color = Color.blue;
                    _titleText.text = "ASPD";
                    break;
                case DropType.HP:
                    _dropTypeImage.sprite = _health;
                    _dropTypeImage.color = Color.green;
                    _backgroundImage.color = Color.green;
                    _titleText.text = "ARMOR";
                    break;
                case DropType.AttackRadius:
                    _dropTypeImage.sprite = _radius;
                    _dropTypeImage.color = Color.yellow;
                    _backgroundImage.color = Color.yellow;
                    _titleText.text = "RADIUS";
                    break;
                case DropType.Speed:
                    _dropTypeImage.sprite = _speed;
                    _dropTypeImage.color = Color.magenta;
                    _backgroundImage.color = Color.magenta;
                    _titleText.text = "SPEED";
                    break;
                default:
                    break;
            }
        }

        private void Invest()
        {
            if (_playerResources.MoneyAmount == 0 || _floatingJoystick.Horizontal != 0 || _floatingJoystick.Vertical != 0) return;

            _timer += Time.deltaTime;
            if (_timer > _timeBetweenInvest)
            {
                _timer = 0;
                _coinsInvested++;
                _sounds.PlaySound(_investingSound);
                ShowAnimation();
                _playerResources.RemoveMoney(1);
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
                _timer = -_timeToNextUpgrade;
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
                    _gettingItemPart.Play();
                    break;
                case DropType.AttackRadius:
                    _upgradePart.transform.position = _playerUpgrade.transform.position + Vector3.up * 2f;
                    _upgradePart.Play();
                    break;
                default:
                    break;
            }
            _playerUpgrade.UpgradeCharacter(this, _dropType);
            _upgradesAmount++;

            if (_dropType == DropType.Clon && _upgradesAmount == _maxClonUpgrades)
            {
                DisablePoint();
            }else if (_upgradesAmount==_maxUpgrades)
            {
                DisablePoint();
            }
        }



        public void DisablePoint()
        {
            _animation.Stop();
            _pointGlow.Stop();

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


