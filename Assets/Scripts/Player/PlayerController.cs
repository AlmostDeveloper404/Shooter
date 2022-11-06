using UnityEngine;
using System;
using Zenject;

namespace Main
{
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
        private int _currentHealth;

        private PlayerBaseState _currentState;
        private PlayerRunningState _playerRunningState;
        private PlayerStayingState _playerStayingState;

        public PlayerRunningState PlayerRunningState { get { return _playerRunningState; } }
        public PlayerStayingState PlayerStayingState { get { return _playerStayingState; } }

        public Action<float> OnFireRateUpgraded;
        public Action<int> OnDamageChanged;
        public Action OnClonCreated;
        public Action<int> OnHealthUpgraded;

        [SerializeField] private int _healthIncreaseModificator;
        [SerializeField] private float _fireRateIncreseModificator;
        [SerializeField] private int _damageIncreaseModificator;

        private HealthBar _healthBar;

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
        }

        private void Start()
        {
            _currentHealth = _maxHealth;
            _healthBar.UpdateUI(_maxHealth, _currentHealth);
        }

        private void OnEnable()
        {
            OnFireRateUpgraded += IncreaseFireRate;
            OnHealthUpgraded += IncreaseHP;
        }

        private void OnDisable()
        {
            OnFireRateUpgraded -= IncreaseFireRate;
            OnHealthUpgraded -= IncreaseHP;
        }


        public void UpgradeCharacter(DropType dropType)
        {
            switch (dropType)
            {
                case DropType.Damage:
                    OnDamageChanged?.Invoke(_damageIncreaseModificator);
                    break;
                case DropType.Clon:
                    OnClonCreated?.Invoke();
                    break;
                case DropType.FireRate:
                    OnFireRateUpgraded?.Invoke(_fireRateIncreseModificator);
                    break;
                case DropType.HP:
                    OnHealthUpgraded?.Invoke(_healthIncreaseModificator);
                    break;
                default:
                    break;
            }
        }

        private void IncreaseHP(int amount)
        {
            _maxHealth += amount;
            UpdateBehaviour();
        }

        private void IncreaseFireRate(float amount)
        {
            _shootingRate -= amount;
            UpdateBehaviour();
        }

        public void UpdateBehaviour()
        {
            _currentHealth = _maxHealth;

            _playerStayingState = new PlayerStayingState(_joystick, _layerMask, _rayMask, _detectionRadius, _animator, _rigidbody, _shootingRate, _activeWeapon);
            _playerRunningState = new PlayerRunningState(_joystick, _animator, _speed, _rotationSpeed, _rigidbody);
            _currentState = _playerStayingState;
            _currentState?.EntryState(this);

            _healthBar.UpdateUI(_maxHealth, _currentHealth);
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


