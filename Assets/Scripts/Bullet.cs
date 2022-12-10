using System;
using UnityEngine;

namespace Main
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour, IPoolable<Bullet>
    {
        private Action<Bullet> _returnToPull;

        [SerializeField] private float _speed;
        private Rigidbody _rigidbody;

        private Vector3 _direction;
        private int _damage;

        [SerializeField] private ParticleSystem[] _bulletImpacts;
        [SerializeField] private GameObject[] _trails;

        private bool _isLaunched = false;

        private int _damageUpgrades;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            foreach (var item in _bulletImpacts)
            {
                item.transform.parent = null;
            }
        }

        private void OnDisable()
        {
            ReturnToPool();
        }

        public void Initialize(Action<Bullet> returnAction)
        {
            _isLaunched = false;
            _returnToPull = returnAction;
        }

        public void ReturnToPool()
        {
            _returnToPull?.Invoke(this);
        }

        private void FixedUpdate()
        {
            if (!_isLaunched) return;

            _rigidbody.velocity = _direction.normalized * _speed;
        }

        public void Launch(Transform enemy, int damage, int damageProgression)
        {
            _damageUpgrades = damageProgression;

            EnableProgressionParticles(_damageUpgrades);

            _damage = damage;

            _direction = enemy.position + new Vector3(0f, 1f, 0f) - transform.position;

            transform.rotation = Quaternion.LookRotation(_direction.normalized);
            _isLaunched = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            ITakeDamage damagable = collision.collider.GetComponent<ITakeDamage>();
            if (damagable != null)
            {
                damagable.TakeDamage(_damage);
            }
            ApplyDestruction(collision);
        }

        private void EnableProgressionParticles(int level)
        {
            int maxArrayIndex = _trails.Length - 1;
            if (level >= maxArrayIndex)
            {
                level = maxArrayIndex;
            }

            for (int i = 0; i < _trails.Length; i++)
            {
                _trails[i].SetActive(i == level);
            }
        }

        private void ApplyDestruction(Collision collision)
        {
            int maxArrayIndex = _bulletImpacts.Length - 1;
            if (_damageUpgrades > maxArrayIndex)
            {
                _damageUpgrades = maxArrayIndex;
            }

            _bulletImpacts[_damageUpgrades].transform.position = collision.GetContact(0).point;
            Vector3 direction = (transform.position - collision.transform.position).normalized;

            _bulletImpacts[_damageUpgrades].transform.rotation = Quaternion.LookRotation(direction);

            _bulletImpacts[_damageUpgrades].Play();
            gameObject.SetActive(false);
        }
    }
}

