using System;
using UnityEngine;
using System.Collections;

namespace Main
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class CoinGrx : MonoBehaviour, IPoolable<CoinGrx>
    {
        private Action<CoinGrx> _returnAction;

        private Rigidbody _rigidbody;

        [SerializeField] private float _timeToDisable = 0.5f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Initialize(Action<CoinGrx> returnAction)
        {
            StartCoroutine(DisableCoin());
            _returnAction = returnAction;
        }

        public void ReturnToPool()
        {
            _returnAction?.Invoke(this);
        }

        private IEnumerator DisableCoin()
        {
            yield return Helpers.Helper.GetWait(_timeToDisable);
            gameObject.SetActive(false);
        }

        public void Launch(Vector3 target, Vector3 launchDirection, float angleInDegree)
        {
            _rigidbody.velocity = CulculateVelocity.Culculate(transform.position, target, launchDirection, angleInDegree);
        }

        private void OnDisable()
        {
            ReturnToPool();
        }
    }
}

