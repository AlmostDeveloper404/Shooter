using UnityEngine;
using System;
using UniRx;

namespace Main
{
    [RequireComponent(typeof(Rigidbody))]
    public class Granade : MonoBehaviour, IPoolable<Granade>
    {
        private Action<Granade> _returnToPool;

        private Rigidbody _rigidBody;


        [SerializeField] private float _damageRadius;

        [SerializeField] private int _damage;
        [SerializeField] private float _speed;

        private Transform _target;
        private Vector3 _direction;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        void OnDisable()
        {
            ReturnToPool();
        }

        public void Initialize(Action<Granade> returnAction)
        {
            _returnToPool = returnAction;
        }

        public void ReturnToPool()
        {
            _returnToPool?.Invoke(this);
        }

        public void Throw(Transform target)
        {
            _target = target;
            Throwing();
        }


        private void Throwing()
        {
            transform.parent = null;

            _direction = (_target.transform.position - transform.position + Vector3.up).normalized;
            _rigidBody.AddForce(_direction * _speed);


        }

        private void OnCollisionEnter(Collision other)
        {
            Explode();
        }

        private void Explode()
        {
            Collider[] _allColliders = Physics.OverlapSphere(transform.position, _damageRadius);
            foreach (var collider in _allColliders)
            {
                ITakeDamage takeDamage = collider.GetComponent<ITakeDamage>();
                if (takeDamage != null)
                {
                    takeDamage.TakeDamage(_damage);
                }
            }
            gameObject.SetActive(false);
        }

    }
}

