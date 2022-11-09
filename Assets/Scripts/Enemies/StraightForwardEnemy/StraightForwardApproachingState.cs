using UnityEngine;
using UnityEngine.AI;
using UniRx.Triggers;
using UniRx;

namespace Main
{
    public class StraightForwardApproachingState : EnemyBaseState<StraightForwardEnemy>
    {
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private Collider _readyToAttackCollider;
        private Collider _detectionCollider;
        private LayerMask _rayMask;

        private CompositeDisposable _onTriggerStayDis = new CompositeDisposable();
        private CompositeDisposable _onTriggerExitDis = new CompositeDisposable();
        private CompositeDisposable _onTriggerStayAttackDis = new CompositeDisposable();


        private StraightForwardAttackState _straightForwardAttackState;
        private StraightForwardDetectingState _straightForwardDetectingState;

        private float _runSpeed;
        private Weapon _weapon;

        private float _waitingTime;

        private bool _targetIsVisible = false;

        private StraightForwardPatrollingState _straightForwardPatrollingState;

        public StraightForwardApproachingState(NavMeshAgent navMeshAgent, Animator animator, float runSpeed, float waitingAfterLosingTarget, Collider attackRadiusCollider, Collider detectionCollider, Weapon weapon, LayerMask rayMask, StraightForwardPatrollingState straightForwardPatrollingState)
        {
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _runSpeed = runSpeed;
            _readyToAttackCollider = attackRadiusCollider;
            _rayMask = rayMask;
            _weapon = weapon;
            _waitingTime = waitingAfterLosingTarget;
            _straightForwardPatrollingState = straightForwardPatrollingState;
            _detectionCollider = detectionCollider;
        }

        public override void EntryState(StraightForwardEnemy straightForwardEnemy)
        {
            Debug.Log("Approach");
            _targetIsVisible = true;
            _onTriggerStayDis?.Clear();
            _onTriggerExitDis?.Clear();
            _onTriggerStayAttackDis?.Clear();

            _readyToAttackCollider.OnTriggerStayAsObservable().Where(t => t.GetComponent<Unit>()).Subscribe(_ => Attack(straightForwardEnemy, _.GetComponent<Unit>())).AddTo(_onTriggerStayAttackDis);
            _detectionCollider.OnTriggerStayAsObservable().Where(t => t.GetComponent<Unit>()).Subscribe(_ => TryApproach(_, straightForwardEnemy)).AddTo(_onTriggerStayDis);
            _detectionCollider.OnTriggerExitAsObservable().Where(t => t.GetComponent<Unit>()).Subscribe(_ => LostView()).AddTo(_onTriggerExitDis);
            _navMeshAgent.speed = _runSpeed;
            _animator.SetBool(Animations.Attack, false);
            _animator.SetBool(Animations.Detecting, false);
            _animator.SetBool(Animations.Idle, false);
            _animator.SetBool(Animations.Run, true);
        }


        public override void UpdateState(StraightForwardEnemy straightForwardEnemy)
        {
            if (!_targetIsVisible)
            {
                _onTriggerExitDis?.Clear();
                _onTriggerStayDis?.Clear();
                _onTriggerStayDis?.Clear();
                _straightForwardDetectingState = new StraightForwardDetectingState(_animator, _navMeshAgent, _detectionCollider, _rayMask, _waitingTime, _straightForwardPatrollingState, this);
                straightForwardEnemy.ChangeState(_straightForwardDetectingState);
            }
        }

        private void LostView()
        {
            _targetIsVisible = false;
        }

        private void TryApproach(Collider collider, StraightForwardEnemy straightForwardEnemy)
        {
            _navMeshAgent.SetDestination(collider.transform.position);

            if (!HasDirectView<Unit>.HasView(straightForwardEnemy.transform.position, collider.transform.position, _rayMask))
            {
                _targetIsVisible = false;
                return;
            }
        }

        private void Attack(StraightForwardEnemy enemy, Unit unit)
        {
            if (!HasDirectView<Unit>.HasView(enemy.transform.position, unit.transform.position, _rayMask)) return;

            _onTriggerExitDis?.Clear();
            _onTriggerStayDis?.Clear();
            _onTriggerStayAttackDis?.Clear();
            _straightForwardAttackState = new StraightForwardAttackState(unit, _animator, _readyToAttackCollider, _navMeshAgent, _weapon.FireRate, _runSpeed, _weapon, _rayMask, this);
            enemy.ChangeState(_straightForwardAttackState);
        }
    }
}


