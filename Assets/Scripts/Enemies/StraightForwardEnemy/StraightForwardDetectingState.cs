using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;

namespace Main
{
    public class StraightForwardDetectingState : EnemyBaseState<StraightForwardEnemy>
    {
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;

        private Collider _detectionCollider;

        private StraightForwardPatrollingState _straightForwardPatrollingState;
        private StraightForwardApproachingState _straightForwardApproachingState;

        private LayerMask _rayMask;

        private float _timer;
        private float _timeToDetect;

        private CompositeDisposable _onTriggerEnterDisposable = new CompositeDisposable();

        public StraightForwardDetectingState(Animator animator, NavMeshAgent navMeshAgent, Collider detectionCollider, LayerMask rayMask, float timeToDetect, StraightForwardPatrollingState straightForwardPatrollingState, StraightForwardApproachingState straightForwardApproachingState)
        {
            _animator = animator;
            _navMeshAgent = navMeshAgent;
            _timeToDetect = timeToDetect;
            _detectionCollider = detectionCollider;
            _straightForwardPatrollingState = straightForwardPatrollingState;
            _straightForwardApproachingState = straightForwardApproachingState;
            _rayMask = rayMask;
        }

        public override void EntryState(StraightForwardEnemy straightForwardEnemy)
        {
            _onTriggerEnterDisposable?.Clear();
            
            _detectionCollider.OnTriggerStayAsObservable().Where(t => t.GetComponent<Unit>()).Subscribe(_ => CheckForTarget(straightForwardEnemy, _)).AddTo(_onTriggerEnterDisposable);
        }
        public override void UpdateState(StraightForwardEnemy straightForwardEnemy)
        {
            if (_navMeshAgent.velocity != Vector3.zero) return;

            _animator.SetBool(Animations.Idle, true);
            _animator.SetBool(Animations.Run, false);

            _timer += Time.deltaTime;
            if (_timer > _timeToDetect)
            {
                _timer = 0;
                straightForwardEnemy.ChangeState(_straightForwardPatrollingState);
                _onTriggerEnterDisposable?.Clear();
            }
        }

        private void CheckForTarget(StraightForwardEnemy straightForwardEnemy, Collider target)
        {
            if (!HasDirectView<Unit>.HasView(straightForwardEnemy.transform.position, target.transform.position, _rayMask)) return;

                straightForwardEnemy.ChangeState(_straightForwardApproachingState);
            _onTriggerEnterDisposable?.Clear();
        }

        public override void ExitState(StraightForwardEnemy straightForwardEnemy)
        {
            _onTriggerEnterDisposable?.Clear();
        }

    }
}

