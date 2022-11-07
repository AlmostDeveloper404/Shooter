using UnityEngine;
using System;
using Zenject;

namespace Main
{
    public class PlayerUpgrade : MonoBehaviour
    {

        public event Action<float> OnFireRateUpgraded;
        public event Action<int> OnDamageChanged;
        public event Action<int> OnHealthUpgraded;


        [SerializeField] private int _healthIncreaseModificator;
        [SerializeField] private float _fireRateIncreseModificator;
        [SerializeField] private int _damageIncreaseModificator;

        [SerializeField] private GameObject _clonPrefab;

        private DiContainer _diContainer;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
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
        }

        public void CreateClon(Vector3 pos)
        {
            PlayerClon playerClon = ClonCreator.CreateClon(pos, _clonPrefab, _diContainer);
            playerClon.SetupSpawnPoint(pos);
        }
    }
}

