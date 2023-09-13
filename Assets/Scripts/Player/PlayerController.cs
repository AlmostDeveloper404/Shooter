using UnityEngine;
using System;
using Zenject;
using System.Collections;

namespace Main
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerController : Unit
    {
        [Header("PlayerSettings")]
        private float _detectionRadius;
        private float _speed;
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

        private CutSceneActivator _bossTriggerActivator;
        private Sounds _sounds;

        [SerializeField] private AudioClip _steps;

        private Camera _mainCam;

        [Inject]
        private void Construct(FloatingJoystick floatingJoystick, CutSceneActivator bossTriggerActivator, Sounds sounds)
        {
            _joystick = floatingJoystick;
            _bossTriggerActivator = bossTriggerActivator;
            _sounds = sounds;
        }
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            _playerUpgrade = GetComponent<PlayerUpgrade>();
            _mainCam = Camera.main;
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += GameOver;
            GameManager.OnLevelCompleted += LevelCompleted;
            _playerUpgrade.OnWeaponChanged += WeaponChanged;
            _playerUpgrade.OnRadiusUpgraded += UpgradeRadius;
            _playerUpgrade.OnSpeedUpgraded += UpgradeSpeed;
            _bossTriggerActivator.OnBossFight += DisableController;
            _bossTriggerActivator.OnCutSceneEnded += EnableController;
        }


        private void OnDisable()
        {
            GameManager.OnGameOver -= GameOver;
            GameManager.OnLevelCompleted -= LevelCompleted;
            _playerUpgrade.OnWeaponChanged -= WeaponChanged;
            _playerUpgrade.OnRadiusUpgraded -= UpgradeRadius;
            _playerUpgrade.OnSpeedUpgraded -= UpgradeSpeed;
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
            _playerStayingState = new PlayerStayingState(_joystick, _layerMask, _rayMask, _detectionRadius, _animator, _rigidbody, _activeWeapon, _sounds, _steps, _mainCam);
            _playerRunningState = new PlayerRunningState(_joystick, _animator, _speed, _rotationSpeed, _rigidbody, _sounds, _steps, _mainCam);
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
            StartCoroutine(RestartLevel());
        }


        private void WeaponChanged(Weapon weapon)
        {
            _activeWeapon = weapon;
            UpdateBehaviour();
        }

        private void UpgradeRadius(float amount)
        {
            _detectionRadius = amount;
            UpdateBehaviour();
        }

        private void UpgradeSpeed(float amount)
        {
            _speed = amount;
            UpdateBehaviour();
        }

        private void LevelCompleted()
        {
            _currentState = null;
            _animator.SetBool(Animations.Idle, true);
            _animator.SetBool(Animations.Run, false);
            _speed = 0;
            _rigidbody.velocity = Vector3.zero;
        }

        private IEnumerator RestartLevel()
        {
            yield return Helpers.Helper.GetWait(2f);
            GameManager.Restart();
        }
    }
}


