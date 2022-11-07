using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;

namespace Main
{
    public class ClonApproaching : ClonBaseState
    {
        private Enemy _targetEnemy;
        private NavMeshAgent _navMesh;
        private Animator _animator;
        private Collider _attackRadiusCollider;
        private Weapon _weapon;
        private PlayerController _playerController;

        private CompositeDisposable _onTriggerEnter = new CompositeDisposable();

        private ClonAttackState _clonAttackState;

        private PlayerClon _playerClon;

        public ClonApproaching(Enemy enemy, NavMeshAgent navMeshAgent, Animator animator, Collider collider, Weapon weapon, PlayerController playerController)
        {
            _targetEnemy = enemy;
            _navMesh = navMeshAgent;
            _animator = animator;
            _attackRadiusCollider = collider;
            _weapon = weapon;
            _playerController = playerController;
        }

        public override void EntryState(PlayerClon playerClon)
        {
            _playerClon = playerClon;

            _attackRadiusCollider.OnTriggerEnterAsObservable().Where(t => t.GetComponent<Enemy>()).Subscribe(_ => ChangeToAttackState()).AddTo(_onTriggerEnter);

            _animator.SetBool(Animations.Idle, false);
            _animator.SetBool(Animations.Run, true);
        }

        public override void UpdateState(PlayerClon playerClon)
        {
            _navMesh.SetDestination(_targetEnemy.transform.position);
        }

        private void ChangeToAttackState()
        {
            Debug.Log("Yep");
            _clonAttackState = new ClonAttackState(_animator, _targetEnemy, _weapon, _navMesh,_playerController);
            _playerClon.ChangeClonState(_clonAttackState);
            _onTriggerEnter?.Clear();
        }
    }
}

