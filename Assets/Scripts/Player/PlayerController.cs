using UnityEngine;
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
        [SerializeField] private int _health;

        private PlayerBaseState _currentState;
        private PlayerRunningState _playerRunningState;
        private PlayerStayingState _playerStayingState;

        public PlayerRunningState PlayerRunningState { get { return _playerRunningState; } }
        public PlayerStayingState PlayerStayingState { get { return _playerStayingState; } }


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
            _health -= damage;
            if (_health <= 0)
            {
                Death();
            }
        }

        private void Death()
        {
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


