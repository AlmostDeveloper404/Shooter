using UnityEngine;
using System;
using Zenject;

namespace Main
{
    public enum DropType { Damage, Clon, FireRate, HP, Speed, AttackRadius }

    [Serializable]
    public struct PlayerProgress
    {
        public float FireRate;
        public float AttackRadius;
        public float Speed;
        public int Hp;
        public int Damage;
        public int DamageUpgrades;
        public int HealthUpgrades;
    }

    public class PlayerUpgrade : MonoBehaviour
    {

        public event Action<float> OnFireRateUpgraded;
        public event Action<float> OnRadiusUpgraded;
        public event Action<float> OnSpeedUpgraded;
        public event Action<int, int> OnDamageChanged;
        public event Action<int, int> OnHealthUpgraded;
        public event Action<Weapon> OnWeaponChanged;

        public event Action OnUpgraded;

        private Weapon _playerWeapon;

        [Header("Default Player Values")]
        [SerializeField] private int _defaultHealth;
        [SerializeField] private float _defaultFireRate;
        [SerializeField] private int _defaultDamage;
        [SerializeField] private float _defaultRadius;
        [SerializeField] private float _defaultSpeed;


        [Header("FireRate")]
        [SerializeField] private float _fireRateIncreseModificator;
        private float _currentFireRate;

        [Header("Damage")]
        [SerializeField] private int _damageIncreaseModificator;
        private int _currentDamage;
        private int _damageUpgrades;

        [Header("Clon")]
        [SerializeField] private GameObject _clonPrefab;

        [Header("Health")]
        [SerializeField] private int _healthIncreaseModificator;
        private int _currentHealth;
        private int _healthUpgrades;

        [Header("DetectionRadius")]
        [SerializeField] private float _radiusIncreaseMOdificator;
        private float _currentRadius;

        [Header("Speed")]
        [SerializeField] private float _speedIncreaseModificator;
        private float _currentSpeed;


        private DiContainer _diContainer;
        private ObjectPool<PlayerClon> _playerClonPull;

        private Sounds _sounds;

        [SerializeField] private AudioClip _damageSound;
        [SerializeField] private AudioClip _fireRateSound;
        [SerializeField] private AudioClip _clonSound;
        [SerializeField] private AudioClip _healthSound;

        [SerializeField] private RectTransform _attackRadiusTransform;

        private Animator _animator;

        [Inject]
        private void Construct(DiContainer diContainer, Sounds sounds)
        {
            _diContainer = diContainer;
            _sounds = sounds;
        }

        private void Awake()
        {
            _playerWeapon = GetComponentInChildren<Weapon>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            GameManager.OnLevelCompleted += Save;
        }

        private void OnDisable()
        {
            GameManager.OnLevelCompleted -= Save;
        }
        private void Start()
        {
            LoadProgress();
            _playerClonPull = new ObjectPool<PlayerClon>(_clonPrefab, _diContainer);
            OnWeaponChanged?.Invoke(_playerWeapon);
        }


        private void LoadProgress()
        {
            PlayerProgress playerProgress = SaveLoadProgress.LoadData<PlayerProgress>(UniqSavingId.PlayerProgress);
            if (playerProgress.Equals(default(PlayerProgress)))
            {
                playerProgress.Damage = _defaultDamage;
                playerProgress.FireRate = _defaultFireRate;
                playerProgress.Hp = _defaultHealth;
                playerProgress.Speed = _defaultSpeed;
                playerProgress.AttackRadius = _defaultRadius;
                playerProgress.DamageUpgrades = 0;
                playerProgress.HealthUpgrades = 0;
                SetCharacterData(playerProgress);
            }
            else
            {
                SetCharacterData(playerProgress);
            }
        }

        private void SetCharacterData(PlayerProgress playerProgress)
        {
            _currentHealth = playerProgress.Hp;
            _healthUpgrades = playerProgress.HealthUpgrades;
            OnHealthUpgraded?.Invoke(_currentHealth, _healthUpgrades);

            _currentFireRate = playerProgress.FireRate;
            OnFireRateUpgraded?.Invoke(_currentFireRate);

            _currentDamage = playerProgress.Damage;
            _damageUpgrades = playerProgress.DamageUpgrades;
            OnDamageChanged?.Invoke(_currentDamage, _damageUpgrades);

            _currentRadius = playerProgress.AttackRadius;
            _attackRadiusTransform.sizeDelta = new Vector2(_currentRadius * 2, _currentRadius * 2);
            OnRadiusUpgraded?.Invoke(_currentRadius);

            _currentSpeed = playerProgress.Speed;
            OnSpeedUpgraded?.Invoke(_currentSpeed);
        }

        public void UpgradeCharacter(UpgradePoint upgradePoint, DropType dropType)
        {
            switch (dropType)
            {
                case DropType.Damage:
                    _damageUpgrades++;
                    _currentDamage += _damageIncreaseModificator;
                    OnDamageChanged?.Invoke(_currentDamage, _damageUpgrades);
                    _animator.SetTrigger(Animations.UpgradeSecond);
                    _sounds.PlaySound(_damageSound);
                    break;
                case DropType.Clon:
                    UpgradeClon(upgradePoint);
                    _animator.SetTrigger(Animations.UpgradeFirst);
                    _sounds.PlaySound(_clonSound);
                    break;
                case DropType.FireRate:
                    _currentFireRate -= _fireRateIncreseModificator;
                    OnFireRateUpgraded?.Invoke(_currentFireRate);
                    _sounds.PlaySound(_fireRateSound);
                    _animator.SetTrigger(Animations.UpgradeSecond);
                    break;
                case DropType.HP:
                    _healthUpgrades++;
                    _currentHealth += _healthIncreaseModificator;
                    OnHealthUpgraded?.Invoke(_currentHealth, _healthUpgrades);
                    _sounds.PlaySound(_healthSound);
                    _animator.SetTrigger(Animations.UpgradeSecond);
                    break;
                case DropType.AttackRadius:
                    _currentRadius += _radiusIncreaseMOdificator;
                    _attackRadiusTransform.sizeDelta = new Vector2(_currentRadius * 2, _currentRadius * 2);
                    OnRadiusUpgraded?.Invoke(_currentRadius);
                    _animator.SetTrigger(Animations.UpgradeFirst);
                    break;
                case DropType.Speed:
                    _currentSpeed += _speedIncreaseModificator;
                    OnSpeedUpgraded?.Invoke(_currentSpeed);
                    _animator.SetTrigger(Animations.UpgradeFirst);
                    break;
                default:
                    break;
            }
            OnUpgraded?.Invoke();
        }

        private void Save()
        {
            PlayerProgress playerProgress = new PlayerProgress { Damage = _currentDamage, Hp = _currentHealth, FireRate = _currentFireRate, DamageUpgrades = _damageUpgrades, HealthUpgrades = _healthUpgrades, AttackRadius = _currentRadius, Speed = _currentSpeed };
            SaveLoadProgress.SaveData(playerProgress, UniqSavingId.PlayerProgress);
        }

        private void UpgradeClon(UpgradePoint upgradePoint)
        {
            _playerClonPull.PullZenject(upgradePoint.ClonSpawnPosition);
        }

        [ContextMenu("Delete Saves")]
        private void DeleteSaves()
        {
            SaveLoadProgress.DeleteData(UniqSavingId.PlayerProgress);
        }

        [ContextMenu("UpgradeFireRate")]
        private void UpgradeFireRate()
        {
            UpgradeCharacter(null,DropType.FireRate);
        }
        [ContextMenu("UpgradeDamage")]
        private void UpgradeDamage()
        {
            UpgradeCharacter(null, DropType.Damage);
        }
        [ContextMenu("UpgradeHealth")]
        private void UpgradeHealth()
        {
            UpgradeCharacter(null, DropType.HP);
        }
        [ContextMenu("UpgradeRadius")]
        private void UpgradeRadius()
        {
            UpgradeCharacter(null, DropType.AttackRadius);
        }
        [ContextMenu("UpgradeSpeed")]
        private void UpgradeSpeed()
        {
            UpgradeCharacter(null, DropType.Speed);
        }
    }
}

