using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using System.Collections;

namespace Main
{
    public class Coin : MonoBehaviour, IInteractable, IPoolable<Coin>
    {
        private Action<Coin> _returnAction;

        private Rigidbody _rigidbody;
        private SphereCollider _sphereCollider;

        private CompositeDisposable _onCollisionEnter = new CompositeDisposable();
        private CompositeDisposable _onPulledToPLayer = new CompositeDisposable();

        [SerializeField] private Collider _collider;

        public bool IsForPurchase { get; set; }

        [SerializeField] private float _pullMultiplier = 5f;

        private Sounds _sounds;
        private PlayerResources _playerResources;
        private PlayerController _playerController;

        [SerializeField] private bool _isOnTheGround;

        [SerializeField] private int _coinsAmount = 1;

        [SerializeField] private AudioClip _collectSound;

        [Inject]
        private void Construct(Sounds sounds, PlayerResources playerResources, PlayerController playerController)
        {
            _sounds = sounds;
            _playerResources = playerResources;
            _playerController = playerController;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _sphereCollider = GetComponent<SphereCollider>();
        }

        private void Start()
        {
            if (_isOnTheGround)
            {
                _onPulledToPLayer?.Clear();

                _collider.OnTriggerEnterAsObservable().Where(t => t.GetComponent<PlayerController>()).Subscribe(_ => PullToPlayer()).AddTo(_onPulledToPLayer);
            }
        }

        public void Initialize(Action<Coin> returnAction)
        {
            
            _returnAction = returnAction;

            _sphereCollider.isTrigger = false;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;

            _sphereCollider.OnCollisionEnterAsObservable().Where(t => t.gameObject.layer == LayerMask.NameToLayer("Ground")).Subscribe(_ => FallOnGround()).AddTo(_onCollisionEnter);
            
        }

        public void PullToPlayer()
        {
            StartCoroutine(StartPulling());
        }

        private IEnumerator StartPulling()
        {

            Vector3 startPosision = transform.position;
            for (float i = 0; i < 2f; i += Time.deltaTime * _pullMultiplier)
            {
                Vector3 position = Vector3.Lerp(startPosision, _playerController.transform.position + Vector3.up, i * i);
                transform.position = position;
                yield return null;
            }
        }

        public void Interact()
        {
            _playerResources.AddMoney(_coinsAmount);
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

        private void FallOnGround()
        {
            _sphereCollider.isTrigger = true;
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            _collider.OnTriggerEnterAsObservable().Where(t => t.GetComponent<PlayerController>()).Subscribe(_ => PullToPlayer()).AddTo(_onPulledToPLayer);
        }

        private void OnDisable()
        {
            _onPulledToPLayer?.Clear();
            _onCollisionEnter?.Clear();
            ReturnToPool();
        }

        private void OnDestroy()
        {
            _onCollisionEnter?.Clear();
            _onPulledToPLayer?.Clear();
        }

    }
}

