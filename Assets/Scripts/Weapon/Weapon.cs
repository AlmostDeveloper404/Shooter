using UnityEngine;
using Zenject;

namespace Main
{
    public enum WeaponUser { PlayerArmy, Enemy, Clon }

    public class Weapon : MonoBehaviour, IDoDamage
    {
        private PlayerUpgrade _playerUpgrade;
        private Sounds _sounds;

        [SerializeField] private ParticleSystem[] _shotParticles;
        [SerializeField] private AudioClip[] _shotSounds;
        [SerializeField] private GameObject[] _pistolGrx;

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

            if (_weaponUser != WeaponUser.PlayerArmy) return;

            _playerUpgrade = playerController.GetComponent<PlayerUpgrade>();
        }

        private void OnEnable()
        {
            if (_weaponUser != WeaponUser.PlayerArmy) return;

            _playerUpgrade.OnDamageChanged += UpgradeDamage;
            _playerUpgrade.OnFireRateUpgraded += UpgradeFireRate;
        }

        private void OnDisable()
        {
            if (_weaponUser != WeaponUser.PlayerArmy) return;

            _playerUpgrade.OnDamageChanged -= UpgradeDamage;
            _playerUpgrade.OnFireRateUpgraded -= UpgradeFireRate;
        }

        private void UpgradeFireRate(float amount)
        {
            if (_weaponUser != WeaponUser.PlayerArmy) return;

            _fireRate = amount;
            _fireRate = Mathf.Clamp(_fireRate, _minShootingRate, Mathf.Infinity);
        }

        private void UpgradeDamage(int amount, int upgradeCount)
        {
            if (_weaponUser != WeaponUser.PlayerArmy) return;

            _damageProgression = upgradeCount;
            _damage = amount;

            int lastWeapon = _pistolGrx.Length - 1;

            if (_damageProgression >= lastWeapon)
            {
                SetupWeapon(lastWeapon);
            }
            else
            {
                SetupWeapon(_damageProgression);
            }
        }

        public virtual void Attack(Transform target)
        {
            int lastArrayIndexSounds = _shotSounds.Length - 1;
            int lastArrayIndexParticles = _shotParticles.Length - 1;

            if (_shotSounds.Length > 0)
            {
                AudioClip sound = _damageProgression >= lastArrayIndexSounds ? _shotSounds[lastArrayIndexSounds] : _shotSounds[_damageProgression];
                _sounds.PlaySound(sound);
            }

            if (_shotParticles.Length > 0)
            {
                ParticleSystem particleSystem = _damageProgression >= lastArrayIndexParticles ? _shotParticles[lastArrayIndexParticles] : _shotParticles[_damageProgression];
                particleSystem.Play();
            }
        }
        private void SetupWeapon(int index)
        {
            for (int i = 0; i < _pistolGrx.Length; i++)
            {
                _pistolGrx[i].SetActive(i == index);
            }
        }

        public void SetupWeapon(EnemyOverProgression enemyOverProgression)
        {
            _damage = enemyOverProgression.Damage;
            _fireRate = enemyOverProgression.FireRate;
        }
    }
}


