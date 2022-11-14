using UnityEngine;
using System;
using Zenject;

namespace Main
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerController : Unit
    {
        [Header("PlayerSettings")]
        [SerializeField] private float _detectionRadius;
        [SerializeField] private float _speed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private LayerMask _rayMask;


        private Rigidbody _rigidbody;
        private FloatingJoystick _joystick;
        private Animator _animator;
        private PlayerUpgrade _playerUpgrade;
        private Weapon _activeWeapon;

        private PlayerBaseState _currentState;
        private PlayerRunningState _playerRunningState;
        private PlayerStayingState _playerStayingState;

        public float FireRate { get { return _activeWeapon.FireRate; } }

        public PlayerRunningState PlayerRunningState { get { return _playerRunningState; } }
        public PlayerStayingState PlayerStayingState { get { return _playerStayingState; } }

        public Action<Enemy> OnEnemyDetected;

        private BossTriggerActivator _bossTriggerActivator;

        [Inject]
        private void Construct(FloatingJoystick floatingJoystick, BossTriggerActivator bossTriggerActivator)
        {
            _joystick = floatingJoystick;
            _bossTriggerActivator = bossTriggerActivator;
        }
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            _playerUpgrade = GetComponent<PlayerUpgrade>();
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += GameOver;
            _playerUpgrade.OnWeaponChanged += WeaponChanged;
            _bossTriggerActivator.OnBossFight += DisableController;
            _bossTriggerActivator.OnCutSceneEnded += EnableController;
        }


        private void OnDisable()
        {
            GameManager.OnGameOver -= GameOver;
            _playerUpgrade.OnWeaponChanged -= WeaponChanged;
            _bossTriggerActivator.OnBossFight -= DisableController;
            _bossTriggerActivator.OnCutSceneEnded -= EnableController;
        }

        private void Update()
        {
            _currentState?.Update(this);
        }


        private void FixedUpdate()
        {
            _currentState?.FixedUpdate(this);
        }

        public void ChangeState(PlayerBaseState playerBaseState)
        {
            _currentState = playerBaseState;
            _currentState?.EntryState(this);
        }

        public void UpdateBehaviour()
        {
            _playerStayingState = new PlayerStayingState(_joystick, _layerMask, _rayMask, _detectionRadius, _animator, _rigidbody, _activeWeapon);
            _playerRunningState = new PlayerRunningState(_joystick, _animator, _speed, _rotationSpeed, _rigidbody);
            _currentState = _playerStayingState;
            _currentState?.EntryState(this);
        }

        private void DisableController()
        {
            _rigidbody.velocity = Vector3.zero;
            _animator.SetBool(Animations.Idle, true);
            _animator.SetBool(Animations.Run, false);
            _currentState = null;
        }

        private void EnableController()
        {
            UpdateBehaviour();
        }

        private void GameOver()
        {
            _currentState = null;
        }


        private void WeaponChanged(Weapon weapon)
        {
            _activeWeapon = weapon;
            UpdateBehaviour();
        }


    }
}


