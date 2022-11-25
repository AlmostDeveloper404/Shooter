using UnityEngine;
using System;
using Zenject;

namespace Main
{
    public enum DropType { Damage, Clon, FireRate, HP }


    public class PlayerUpgrade : MonoBehaviour
    {

        public event Action<float> OnFireRateUpgraded;
        public event Action<int, int> OnDamageChanged;
        public event Action<int> OnHealthUpgraded;
        public event Action<Weapon> OnWeaponChanged;

        public event Action OnUpgraded;

        private Weapon _playerWeapon;

        [Header("FireRate")]
        [SerializeField] private float _fireRateIncreseModificator;
        [SerializeField] private int _firerateMaxUpgrades;
        private int _currentFireRateUpgrades;

        [Header("Damage")]
        [SerializeField] private int _damageIncreaseModificator;
        [SerializeField] private int _damageMaxUpgrades;
        private int _currentDamageUpgrades;

        [Header("Clon")]
        [SerializeField] private GameObject _clonPrefab;
        [SerializeField] private int _clonMaxUpgrades;
        private int _currentClonUpgrades;

        [Header("Health")]
        [SerializeField] private int _healthIncreaseModificator;
        [SerializeField] private int _healthMaxUpgrades;
        private int _currentHealthUpgrades;


        private DiContainer _diContainer;
        private ObjectPool<PlayerClon> _playerClonPull;

        private void Awake()
        {
            _playerWeapon = GetComponentInChildren<Weapon>();
        }

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        private void Start()
        {
            _playerClonPull = new ObjectPool<PlayerClon>(_clonPrefab, _diContainer);
            OnWeaponChanged?.Invoke(_playerWeapon);
        }

        public void UpgradeCharacter(UpgradePoint upgradePoint, DropType dropType)
        {
            switch (dropType)
            {
                case DropType.Damage:
                    UpdateUpgradePoint(upgradePoint, ref _currentDamageUpgrades, _damageMaxUpgrades);
                    OnDamageChanged?.Invoke(_damageIncreaseModificator, _currentDamageUpgrades);
                    break;
                case DropType.Clon:
                    UpdateUpgradePoint(upgradePoint, ref _currentClonUpgrades, _clonMaxUpgrades);
                    UpgradeClon(upgradePoint);
                    break;
                case DropType.FireRate:
                    UpdateUpgradePoint(upgradePoint, ref _currentFireRateUpgrades, _firerateMaxUpgrades);
                    OnFireRateUpgraded?.Invoke(_fireRateIncreseModificator);
                    break;
                case DropType.HP:
                    UpdateUpgradePoint(upgradePoint, ref _currentHealthUpgrades, _healthMaxUpgrades);
                    OnHealthUpgraded?.Invoke(_healthIncreaseModificator);
                    break;
                default:
                    break;
            }
            OnUpgraded?.Invoke();
        }

        private void UpdateUpgradePoint(UpgradePoint upgradePoint, ref int currentUpgrades, int maxUpgrades)
        {
            currentUpgrades++;
            if (currentUpgrades == maxUpgrades)
            {
                upgradePoint.DisablePoint();
            }
        }


        private void UpgradeClon(UpgradePoint upgradePoint)
        {
            _playerClonPull.PullZenject(upgradePoint.ClonSpawnPosition);
        }
    }
}

