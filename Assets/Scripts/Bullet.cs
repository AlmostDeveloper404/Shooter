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

        private int _progression;

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
            _progression = damageProgression;

            EnableProgressionParticles(damageProgression);

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



            _bulletImpacts[_progression].transform.position = collision.GetContact(0).point;
            Vector3 direction = (transform.position - collision.transform.position).normalized;

            _bulletImpacts[_progression].transform.rotation = Quaternion.LookRotation(direction);

            _bulletImpacts[_progression].Play();
            gameObject.SetActive(false);
        }

        private void EnableProgressionParticles(int level)
        {
            for (int i = 0; i < _trails.Length; i++)
            {
                _trails[i].SetActive(i == level);
            }
        }
    }
}

