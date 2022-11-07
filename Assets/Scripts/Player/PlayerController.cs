using UnityEngine;
using System;
using Zenject;

namespace Main
{
    [Serializable]
    public struct PlayerStats
    {
        public float FireRate;
        public float Health;
    }

    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour, ITakeDamage
    {
        private Rigidbody _rigidbody;
        private FloatingJoystick _joystick;


        [Header("PlayerSettings")]
        [SerializeField] private float _detectionRadius;
        [SerializeField] private float _speed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private LayerMask _rayMask;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _shootingRate;
        [SerializeField] private Weapon _activeWeapon;
        [SerializeField] private int _maxHealth;
        [SerializeField] private float _minShootingRate = 0.1f;

        private PlayerStats _currentStats;
        public PlayerStats PlayerStats { get { return _currentStats; } }

        private int _currentHealth;

        private PlayerBaseState _currentState;
        private PlayerRunningState _playerRunningState;
        private PlayerStayingState _playerStayingState;

        public PlayerRunningState PlayerRunningState { get { return _playerRunningState; } }
        public PlayerStayingState PlayerStayingState { get { return _playerStayingState; } }

        public Action<Enemy> OnEnemyDetected;

        private HealthBar _healthBar;
        private PlayerUpgrade _playerUpgrade;

        [Inject]
        private void Construct(FloatingJoystick floatingJoystick)
        {
            _joystick = floatingJoystick;
            _rigidbody = GetComponent<Rigidbody>();
            _playerStayingState = new PlayerStayingState(_joystick, _layerMask, _rayMask, _detectionRadius, _animator, _rigidbody, _shootingRate, _activeWeapon);
            _playerRunningState = new PlayerRunningState(_joystick, _animator, _speed, _rotationSpeed, _rigidbody);
            _currentState = _playerStayingState;
            _currentState?.EntryState(this);
        }
        private void Awake()
        {
            _healthBar = GetComponentInChildren<HealthBar>();
            _playerUpgrade = GetComponent<PlayerUpgrade>();
        }

        private void OnEnable()
        {
            _playerUpgrade.OnFireRateUpgraded += FireRateUpgraded;
            _playerUpgrade.OnHealthUpgraded += HealthUpgraded;
        }

        private void OnDisable()
        {
            _playerUpgrade.OnFireRateUpgraded -= FireRateUpgraded;
            _playerUpgrade.OnHealthUpgraded -= HealthUpgraded;
        }


        private void Start()
        {
            _currentHealth = _maxHealth;
            _healthBar.UpdateUI(_maxHealth, _currentHealth);
            PlayerResources.AddMoney(10000);
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
        private void HealthUpgraded(int amount)
        {
            _maxHealth += amount;
            UpdateBehaviour();
        }

        private void FireRateUpgraded(float amount)
        {
            _shootingRate -= amount;
            _shootingRate = Mathf.Clamp(_shootingRate, _minShootingRate, Mathf.Infinity);
            UpdateBehaviour();
        }

        public void UpdateBehaviour()
        {
            _currentHealth = _maxHealth;

            _currentStats = new PlayerStats() { FireRate = _shootingRate, Health = _currentHealth };
            _playerStayingState = new PlayerStayingState(_joystick, _layerMask, _rayMask, _detectionRadius, _animator, _rigidbody, _shootingRate, _activeWeapon);
            _playerRunningState = new PlayerRunningState(_joystick, _animator, _speed, _rotationSpeed, _rigidbody);
            _currentState = _playerStayingState;
            _currentState?.EntryState(this);

            _healthBar.UpdateUI(_maxHealth, _currentHealth);
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            _healthBar.UpdateUI(_maxHealth, _currentHealth);
            if (_currentHealth <= 0)
            {
                Death();
            }
        }

        private void Death()
        {
            _healthBar.gameObject.SetActive(false);
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _animator.applyRootMotion = true;
            GameManager.ChangeGameState(GameState.GameOver);
            _animator.SetTrigger(Animations.Death);
            _currentState = null;
            enabled = false;
        }
    }
}


