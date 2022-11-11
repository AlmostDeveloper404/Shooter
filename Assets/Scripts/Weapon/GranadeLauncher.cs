using UnityEngine;

namespace Main
{
    public class GranadeLauncher : Weapon
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _rocketPref;

        private ObjectPool<Rocket> _rocketPool;

        [SerializeField] private float _angle;

        private void Start()
        {
            _rocketPool = new ObjectPool<Rocket>(_rocketPref);
        }

        private void Update()
        {
            _spawnPoint.localEulerAngles = new Vector3(-_angle, 0f, 0f);
        }
        public override void Attack(Transform target)
        {
            Rocket rocket = _rocketPool.Pull(_spawnPoint.position, _spawnPoint.rotation);
            rocket.Launch(target.position, _spawnPoint.forward, _angle);
        }
    }
}

