using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.AI;

namespace Main
{
    public class StraightForwardAttackState : EnemyBaseState<StraightForwardEnemy>
    {
        private Animator _animator;
        private Collider _attackRadius;
        private NavMeshAgent _navMeshAgent;
        private float _attackRate;
        private Weapon _enemyWeapon;
        private Vector3 _lookDirection;
        private LayerMask _layerMask;

        private float _runSpeed;

        private Unit _targetUnit;

        private float _timer;

        private CompositeDisposable _onTriggerExitDis = new CompositeDisposable();

        private StraightForwardApproachingState _straightForwardApproachingState;

        public StraightForwardAttackState(Unit targetUnit, Animator animator, Collider attackRadiusCollider,NavMeshAgent navMeshAgent, float attackRate, float runSpeed, Weapon weapon, LayerMask layerMask, StraightForwardApproachingState straightForwardApproachingState)
        {
            _layerMask = layerMask;
            _animator = animator;
            _attackRadius = attackRadiusCollider;
            _navMeshAgent = navMeshAgent;
            _attackRate = attackRate;
            _enemyWeapon = weapon;
            _targetUnit = targetUnit;
            _runSpeed = runSpeed;
            _straightForwardApproachingState = straightForwardApproachingState;
        }

        public override void EntryState(StraightForwardEnemy straightForwardEnemy)
        {
            Debug.Log("Attack");
            _onTriggerExitDis?.Clear();
            _animator.SetBool(Animations.Attack, true);
            _animator.SetBool(Animations.Idle, false);
            _animator.SetBool(Animations.Detecting, false);
            _animator.SetBool(Animations.Run, false);

            _timer = 0;
            _attackRadius.OnTriggerExitAsObservable().Where(t => t.gameObject.GetComponent<Unit>()).Subscribe(_ => AttackRadiusExit(straightForwardEnemy, _)).AddTo(_onTriggerExitDis);
            _navMeshAgent.speed = 0;
        }
        public override void UpdateState(StraightForwardEnemy straightForwardEnemy)
        {
            if (!HasDirectView<Unit>.HasView(straightForwardEnemy.transform.position, _targetUnit.transform.position, _layerMask))
            {
                _onTriggerExitDis?.Clear();
                _navMeshAgent.SetDestination(_targetUnit.transform.position);
                straightForwardEnemy.ChangeState(_straightForwardApproachingState);
            }

            _lookDirection = _targetUnit.transform.position - straightForwardEnemy.transform.position;
            straightForwardEnemy.transform.rotation = Quaternion.LookRotation(_lookDirection.normalized);

            _timer += Time.deltaTime;
            if (_timer > _attackRate)
            {
                _timer = 0;
                _enemyWeapon.Attack(_targetUnit.transform);
            }
        }

        private void AttackRadiusExit(StraightForwardEnemy straightForwardEnemy, Collider collider)
        {
            _onTriggerExitDis?.Clear();
            _navMeshAgent.SetDestination(collider.transform.position);
            straightForwardEnemy.ChangeState(_straightForwardApproachingState);
        }

    }
}


