using System;
using UnityEngine;
using UnityEngine.AI;

namespace Main
{
    public class ClonEscortState : ClonBaseState
    {
        private NavMeshAgent _navMeshAgent;
        private PlayerController _playerController;
        private Animator _animator;
        private Collider _attackRadiusCollider;
        private Weapon _weapon;

        private PlayerClon _playerClon;

        private bool _isStopped = false;

        private ClonApproaching _clonApproachingState;

        public ClonEscortState(PlayerController playerController, NavMeshAgent navMeshAgent, Animator animator, Collider collider, Weapon weapon)
        {
            _playerController = playerController;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _attackRadiusCollider = collider;
            _weapon = weapon;
        }

        public override void EntryState(PlayerClon playerClon)
        {
            _playerClon = playerClon;

            _playerController.OnEnemyDetected += ChangeState;

            _animator.SetBool(Animations.Idle, true);
            _animator.SetBool(Animations.Run, false);
        }

        public override void UpdateState(PlayerClon playerClon)
        {
            _navMeshAgent.SetDestination(_playerController.transform.position);
            if (_navMeshAgent.velocity == Vector3.zero && !_isStopped)
            {
                _isStopped = true;
                _animator.SetBool(Animations.Idle, true);
                _animator.SetBool(Animations.Run, false);
            }

            if (_navMeshAgent.velocity != Vector3.zero && _isStopped)
            {
                _isStopped = false;
                _animator.SetBool(Animations.Idle, false);
                _animator.SetBool(Animations.Run, true);
            }
        }

        private void ChangeState(Enemy enemy)
        {
            _clonApproachingState = new ClonApproaching(enemy, _navMeshAgent, _animator, _attackRadiusCollider, _weapon,_playerController);
            _playerClon.ChangeClonState(_clonApproachingState);
            _playerController.OnEnemyDetected -= ChangeState;
        }
    }
}

