using UnityEngine;
using Zenject;

namespace Main
{
    public class Weapon : MonoBehaviour, IDoDamage
    {
        private PlayerUpgrade _playerUpgrade;

        [SerializeField] private int _damage;

        public int Damage { get { return _damage; } }

        [Inject]
        private void Construct(PlayerController playerController)
        {
            _playerUpgrade = playerController.GetComponent<PlayerUpgrade>();
        }

        private void OnEnable()
        {
            _playerUpgrade.OnDamageChanged += UpgradeDamage;
        }

        private void OnDisable()
        {
            _playerUpgrade.OnDamageChanged -= UpgradeDamage;
        }

        private void UpgradeDamage(int amount)
        {
            _damage += amount;
        }

        public virtual void Attack(Transform enemy)
        {

        }
    }
}


