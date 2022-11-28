using UnityEngine;
using Zenject;

namespace Main
{
    public enum WeaponUser { PlayerArmy, Enemy }

    public class Weapon : MonoBehaviour, IDoDamage
    {
        private PlayerUpgrade _playerUpgrade;
        private Sounds _sounds;

        [SerializeField] private ParticleSystem[] _shotParticles;
        [SerializeField] private AudioClip[] _shotSounds;

        [SerializeField] private int _damage;
        [SerializeField] private float _fireRate;

        [SerializeField] private float _minShootingRate;

        public int Damage { get { return _damage; } }
        public float FireRate { get { return _fireRate; } }

        private int _damageProgression = 0;
        public int GetDamageProgression { get { return _damageProgression; } }

        [SerializeField] private WeaponUser _weaponUser;

        [Inject]
        private void Construct(PlayerController playerController, Sounds sounds)
        {
            _sounds = sounds;

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

        private void UpgradeDamage(int amount, int upgradeCount)
        {
            if (_weaponUser == WeaponUser.Enemy) return;

            _damageProgression = upgradeCount;
            _damage += amount;
        }

        public virtual void Attack(Transform target)
        {
            if (_shotSounds.Length > 0)
                _sounds.PlaySound(_shotSounds[_damageProgression]);

            if (_shotParticles.Length > 0)
                _shotParticles[_damageProgression]?.Play();
        }
    }
}


