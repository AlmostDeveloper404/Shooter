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

        private CompositeDisposable _onTriggerEnter = new CompositeDisposable();

        private ClonAttackState _clonAttackState;

        private PlayerClon _playerClon;
        private LayerMask _enemyMask;

        public ClonApproaching(Enemy enemy, NavMeshAgent navMeshAgent, Animator animator, Collider collider, Weapon weapon, LayerMask enemyMask)
        {
            _targetEnemy = enemy;
            _navMesh = navMeshAgent;
            _animator = animator;
            _attackRadiusCollider = collider;
            _weapon = weapon;
            _enemyMask = enemyMask;
        }

        public override void EntryState(PlayerClon playerClon)
        {
            _onTriggerEnter?.Clear();

            _playerClon = playerClon;

            _navMesh.speed = _playerClon.Speed;
            _attackRadiusCollider.OnTriggerStayAsObservable().Where(t => t.gameObject.GetComponent<Enemy>()).Subscribe(_ => TryChangeToAttackState()).AddTo(_onTriggerEnter);

            _animator.SetBool(Animations.Idle, false);
            _animator.SetBool(Animations.Run, true);
        }

        public override void UpdateState(PlayerClon playerClon)
        {
            if (_targetEnemy.IsDead)
            {
                playerClon.ChangeClonState(playerClon.ClonEscortState);
                _onTriggerEnter?.Clear();
            }

            _navMesh.SetDestination(_targetEnemy.transform.position);
        }

        private void TryChangeToAttackState()
        {
            Debug.Log(HasDirectView<Enemy>.HasView(_playerClon.transform.position, _targetEnemy.transform.position, _enemyMask));
            if (!HasDirectView<Enemy>.HasView(_playerClon.transform.position, _targetEnemy.transform.position, _enemyMask)) return;

            _onTriggerEnter?.Clear();

            _clonAttackState = new ClonAttackState(_animator, _targetEnemy, _weapon, _navMesh, _enemyMask, _attackRadiusCollider);
            _playerClon.ChangeClonState(_clonAttackState);
        }
    }
}

