using System;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Main
{
    public class PlayerClon : Unit, IPoolable<PlayerClon>
    {
        private NavMeshAgent _navMesh;
        private ClonBaseState _currentState;

        private ClonEscortState _clonEscortState;

        public ClonEscortState ClonEscortState { get { return _clonEscortState; } }

        private Action<PlayerClon> _returnAction;

        [SerializeField] private Weapon _weapon;

        private PlayerController _playerController;

        private Animator _animator;

        private float _navMeshSpeed;

        [SerializeField] private LayerMask _enemyMask;

        private Rigidbody _rigidbody;
        private FloatingJoystick _joystick;

        public float Speed { get { return _navMeshSpeed; } }

        [SerializeField] private Collider _attackRadiusCollider;

        [Inject]
        private void Construct(PlayerController playerController, FloatingJoystick floatingJoystick)
        {
            _joystick = floatingJoystick;
            _playerController = playerController;
            _animator = GetComponentInChildren<Animator>();
        }

        private void Awake()
        {
            _navMesh = GetComponent<NavMeshAgent>();
            _rigidbody = GetComponent<Rigidbody>();

        }

        private void Start()
        {
            _navMeshSpeed = _navMesh.speed;
            UpdateClonBehaivior();
        }

        public void Initialize(Action<PlayerClon> returnAction)
        {
            _returnAction = returnAction;
        }

        public void ReturnToPool()
        {
            _returnAction?.Invoke(this);
        }


        private void Update()
        {
            _currentState?.UpdateState(this);
        }

        public void ChangeClonState(ClonBaseState newState)
        {
            _currentState = newState;
            _currentState.EntryState(this);
        }

        private void UpdateClonBehaivior()
        {
            _clonEscortState = new ClonEscortState(_playerController, _navMesh, _animator, _attackRadiusCollider, _weapon, _enemyMask, _joystick, _rigidbody);
            _currentState = _clonEscortState;
            _currentState?.EntryState(this);
        }

        public void Death()
        {
            _currentState = null;
            enabled = false;
        }
    }
}

