using UnityEngine;
using System;
using Zenject;

namespace Main
{
    public enum DropType { Damage, Clon, FireRate, HP }


    public class PlayerUpgrade : MonoBehaviour
    {

        public event Action<float> OnFireRateUpgraded;
        public event Action<int> OnDamageChanged;
        public event Action<int> OnHealthUpgraded;
        public event Action<Weapon> OnWeaponChanged;

        public event Action OnUpgraded;

        private Weapon _playerWeapon;

        [SerializeField] private int _healthIncreaseModificator;
        [SerializeField] private float _fireRateIncreseModificator;
        [SerializeField] private int _damageIncreaseModificator;

        [SerializeField] private GameObject _clonPrefab;

        private DiContainer _diContainer;

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
            OnWeaponChanged?.Invoke(_playerWeapon);
        }

        public void UpgradeCharacter(UpgradePoint upgradePoint, DropType dropType)
        {
            switch (dropType)
            {
                case DropType.Damage:
                    OnDamageChanged?.Invoke(_damageIncreaseModificator);
                    break;
                case DropType.Clon:
                    CreateClon(upgradePoint.ClonSpawnPosition);
                    break;
                case DropType.FireRate:
                    OnFireRateUpgraded?.Invoke(_fireRateIncreseModificator);
                    break;
                case DropType.HP:
                    OnHealthUpgraded?.Invoke(_healthIncreaseModificator);
                    break;
                default:
                    break;
            }
            OnUpgraded?.Invoke();
        }

        public void CreateClon(Vector3 pos)
        {
            PlayerClon playerClon = ClonCreator.CreateClon(pos, _clonPrefab, _diContainer);
            playerClon.Setup(pos);
        }
    }
}

