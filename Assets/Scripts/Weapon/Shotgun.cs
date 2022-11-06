using UnityEngine;

namespace Main
{
    public class Shotgun : Weapon
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _bulletPref;

        private ObjectPool<Bullet> _bulletPull;

        private void Start()
        {
            _bulletPull = new ObjectPool<Bullet>(_bulletPref);
        }

        public override void Attack(Transform damagable)
        {
            Bullet bullet = _bulletPull.Pull(_spawnPoint.position);
            bullet.Launch(damagable, Damage);
        }
    }
}

