using System;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Main
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerClon : MonoBehaviour, IPoolable<PlayerClon>
    {
        private NavMeshAgent _navMesh;
        private ClonBaseState _currentState;

        private ClonEscortState _clonEscortState;

        private Action<PlayerClon> _returnAction;

        [SerializeField] private Weapon _weapon;

        private PlayerController _playerController;
        private Vector3 _spawnPoint;

        private Animator _animator;

        [SerializeField] private Collider _attackRadiusCollider;

        [Inject]
        private void Construct(PlayerController playerController)
        {
            _playerController = playerController;
            _animator = GetComponentInChildren<Animator>();
        }

        private void Awake()
        {
            _navMesh = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _clonEscortState = new ClonEscortState(_playerController, _navMesh, _animator, _attackRadiusCollider, _weapon);
            _currentState = _clonEscortState;
            _currentState?.EntryState(this);
        }

        public void SetupSpawnPoint(Vector3 point)
        {
            _spawnPoint = point;
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

        private void Death()
        {
            _playerController.GetComponent<PlayerUpgrade>().CreateClon(_spawnPoint);
        }
    }
}

