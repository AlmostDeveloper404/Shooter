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

        private bool _isLaunched = false;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
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

        public void Launch(Transform enemy, int damage)
        {
            _damage = damage;

            _direction = enemy.position + new Vector3(0f, 1f, 0f) - transform.position;
            _isLaunched = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            ITakeDamage damagable = collision.collider.GetComponent<ITakeDamage>();
            if (damagable != null)
            {
                damagable.TakeDamage(_damage);
                gameObject.SetActive(false);
            }

            gameObject.SetActive(false);
        }
    }
}

