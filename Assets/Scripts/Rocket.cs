using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

namespace Main
{
    [RequireComponent(typeof(Rigidbody))]
    public class Rocket : MonoBehaviour, IPoolable<Rocket>
    {
        private Action<Rocket> _returnAction;
        private Rigidbody _rigidbody;

        private CapsuleCollider _capsuleCollider;
        [SerializeField] private Transform _graphicSphere;

        private CompositeDisposable _onCollisionEnterDis = new CompositeDisposable();

        [SerializeField] private float _radius;
        [SerializeField] private int _damage;

        [SerializeField] private float _explosiveForce;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
        }

        public void Initialize(Action<Rocket> returnAction)
        {
            StartCoroutine(GRXAnim());
            _capsuleCollider.OnCollisionEnterAsObservable().Subscribe(_ => Detonate()).AddTo(_onCollisionEnterDis);
            _returnAction = returnAction;

        }

        public void ReturnToPool()
        {
            _returnAction?.Invoke(this);
        }

        public void Launch(Vector3 target, Vector3 launchDirection, float angle)
        {
            _rigidbody.velocity = CulculateVelocity.Culculate(transform.position, target, launchDirection, angle);
        }

        private void Detonate()
        {
            Explode();
            _onCollisionEnterDis?.Clear();
        }

        private void Explode()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
            foreach (var collider in colliders)
            {
                ITakeDamage takeDamage = collider.GetComponent<ITakeDamage>();
                if (takeDamage != null)
                {
                    takeDamage.TakeDamage(_damage);
                    continue;
                }
                Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
                if (rigidbody)
                {
                    rigidbody.AddExplosionForce(_explosiveForce, transform.position, _radius);
                }
            }

            gameObject.SetActive(false);
        }

        private IEnumerator GRXAnim()
        {
            for (float i = 0; i < 1; i += Time.deltaTime)
            {
                _graphicSphere.localScale = new Vector3(i, i, i) * _radius;
                yield return null;
            }
        }

        private void OnDisable()
        {
            ReturnToPool();
        }

    }
}


