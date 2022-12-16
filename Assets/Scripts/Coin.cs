using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace Main
{
    public class Coin : MonoBehaviour, IInteractable, IPoolable<Coin>
    {
        private Action<Coin> _returnAction;

        private Rigidbody _rigidbody;
        private SphereCollider _sphereCollider;

        private CompositeDisposable _onCollisionEnter = new CompositeDisposable();

        public bool IsForPurchase { get; set; }

        private Sounds _sounds;
        private PlayerResources _playerResources;

        [SerializeField] private AudioClip _collectSound;

        [Inject]
        private void Construct(Sounds sounds,PlayerResources playerResources)
        {
            _sounds = sounds;
            _playerResources = playerResources;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _sphereCollider = GetComponent<SphereCollider>();
        }


        public void Initialize(Action<Coin> returnAction)
        {
            _returnAction = returnAction;

            _sphereCollider.isTrigger = false;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;

            _sphereCollider.OnCollisionEnterAsObservable().Where(t => t.gameObject.layer == LayerMask.NameToLayer("Ground")).Subscribe(_ => CheckCollision()).AddTo(_onCollisionEnter);
        }

        public void Interact()
        {
            _playerResources.AddMoney(1);
            gameObject.SetActive(false);
            _sounds.PlaySound(_collectSound);
        }

        public void ReturnToPool()
        {
            _returnAction?.Invoke(this);
        }

        public void Launch(Vector3 target, Vector3 launchDirection, float angleInDegree)
        {
            _rigidbody.velocity = CulculateVelocity.Culculate(transform.position, target, launchDirection, angleInDegree);
        }

        private void CheckCollision()
        {
            _sphereCollider.isTrigger = true;
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }

        private void OnDisable()
        {
            ReturnToPool();
        }
    }
}

