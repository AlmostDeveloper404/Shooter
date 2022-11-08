using System;
using UnityEngine;
using Zenject;

namespace Main
{
    public enum WeaponUser { PlayerArmy, Enemy }

    public class Weapon : MonoBehaviour, IDoDamage
    {
        private PlayerUpgrade _playerUpgrade;

        [SerializeField] private int _damage;
        [SerializeField] private float _fireRate;

        [SerializeField] private float _minShootingRate;

        public int Damage { get { return _damage; } }
        public float FireRate { get { return _fireRate; } }

        [SerializeField] private WeaponUser _weaponUser;

        [Inject]
        private void Construct(PlayerController playerController)
        {
            if (_weaponUser == WeaponUser.Enemy) return;

            _playerUpgrade = playerController.GetComponent<PlayerUpgrade>();
        }

        private void OnEnable()
        {
            if (_weaponUser == WeaponUser.Enemy) return;

            _playerUpgrade.OnDamageChanged += UpgradeDamage;
            _playerUpgrade.OnFireRateUpgraded += UpgradeFireRate;
        }

        private void OnDisable()
        {
            if (_weaponUser == WeaponUser.Enemy) return;

            _playerUpgrade.OnDamageChanged -= UpgradeDamage;
            _playerUpgrade.OnFireRateUpgraded -= UpgradeFireRate;
        }

        private void UpgradeFireRate(float amount)
        {
            if (_weaponUser == WeaponUser.Enemy) return;

            _fireRate -= amount;
            _fireRate = Mathf.Clamp(_fireRate, _minShootingRate, Mathf.Infinity);
        }

        private void UpgradeDamage(int amount)
        {
            if (_weaponUser == WeaponUser.Enemy) return;

            _damage += amount;
        }

        public virtual void Attack(Transform enemy)
        {

        }
    }
}


