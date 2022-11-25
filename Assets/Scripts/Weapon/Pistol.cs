using UnityEngine;

namespace Main
{
    public class Pistol : Weapon
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _bulletPref;

        private ObjectPool<Bullet> _bulletPull;

        [SerializeField] private ParticleSystem[] _shootParticles;



        private void Start()
        {
            _bulletPull = new ObjectPool<Bullet>(_bulletPref);
        }

        public override void Attack(Transform damagable)
        {
            _shootParticles[GetDamageProgression].Play();

            Bullet bullet = _bulletPull.Pull(_spawnPoint.position);
            bullet.Launch(damagable, Damage, GetDamageProgression);
        }
    }
}

