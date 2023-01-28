using UnityEngine;
using Zenject;

namespace Main
{
    public class GranadeLauncher : Weapon
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _rocketPref;

        private ObjectPool<Rocket> _rocketPool;

        [SerializeField] private float _angle;

        private DiContainer _diContainer;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        private void Start()
        {
            _rocketPool = new ObjectPool<Rocket>(_rocketPref, _diContainer);
        }

        private void Update()
        {
            _spawnPoint.localEulerAngles = new Vector3(-_angle, 0f, 0f);
        }
        public override void Attack(Transform target)
        {
            base.Attack(target);

            Rocket rocket = _rocketPool.PullZenject(_spawnPoint.position, _spawnPoint.rotation);

            rocket.Launch(target.position, _spawnPoint.forward, _angle, Damage);
        }
    }
}

