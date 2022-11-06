using System;
using UnityEngine;

namespace Main
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class CoinGrx : MonoBehaviour, IPoolable<CoinGrx>
    {
        private Action<CoinGrx> _returnAction;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Initialize(Action<CoinGrx> returnAction)
        {
            _returnAction = returnAction;
        }

        public void ReturnToPool()
        {
            _returnAction?.Invoke(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            UpgradePoint upgradePoint = other.GetComponent<UpgradePoint>();
            if (upgradePoint)
            {
                gameObject.SetActive(false);
            }
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

