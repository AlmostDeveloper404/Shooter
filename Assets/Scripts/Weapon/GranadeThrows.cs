using UnityEngine;

namespace Main
{
    public class GranadeThrows : Weapon
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _granade;
        private ObjectPool<Granade> _bulletPull;

        private void Start()
        {
            _bulletPull = new ObjectPool<Granade>(_granade);
        }

        public override void Attack(Transform target)
        {
            Granade granade = _bulletPull.Pull(_spawnPoint.position);
            granade.Throw(target);
        }
    }
}


