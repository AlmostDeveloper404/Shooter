using UnityEngine;
using UnityEngine.AI;
using UniRx.Triggers;
using UniRx;

namespace Main
{
    public class StraightForwardPatrollingState : EnemyBaseState<StraightForwardEnemy>
    {
        private Transform[] _points;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        private int _currentPatrollingPointIndex;
        private bool _isEnd = false;

        private float _timer;
        private float _patrollingSpeed;
        private float _timeToStay;

        private CompositeDisposable _detectionRadius = new CompositeDisposable();
        private Collider _detectionCollider;

        public StraightForwardPatrollingState(Transform container, NavMeshAgent navMeshAgent, Animator animator, float timeToStay, Collider detectionRadiusCollider, float partolSpeed)
        {


            _detectionCollider = detectionRadiusCollider;
            _timeToStay = timeToStay;
            _animator = animator;
            _navMeshAgent = navMeshAgent;
            _patrollingSpeed = partolSpeed;

            _points = new Transform[container.childCount];
            for (int i = 0; i < _points.Length; i++)
            {
                _points[i] = container.GetChild(i);
            }
        }

        public override void EntryState(StraightForwardEnemy straightForwardEnemy)
        {
            _animator.SetBool(Animations.Idle, false);
            _animator.SetBool(Animations.Detecting, true);

            _navMeshAgent.speed = _patrollingSpeed;

            _detectionRadius?.Clear();

            _detectionCollider.OnTriggerEnterAsObservable().Where(t => t.gameObject.GetComponent<PlayerController>()).Subscribe(_ => CheckForPlayer(straightForwardEnemy)).AddTo(_detectionRadius);
            _currentPatrollingPointIndex = 0;
            _navMeshAgent.SetDestination(_points[_currentPatrollingPointIndex].position);

        }
        public override void UpdateState(StraightForwardEnemy straightForwardEnemy)
        {
            if (!_isEnd) return;

            _timer += Time.deltaTime;
            if (_timer > _timeToStay)
            {
                _timer = 0;
                _isEnd = false;
                _animator.SetBool(Animations.Detecting, true);
                _animator.SetBool(Animations.Idle, false);
                _navMeshAgent.speed = _patrollingSpeed;
                _currentPatrollingPointIndex = _currentPatrollingPointIndex == _points.Length - 1 ? 0 : _currentPatrollingPointIndex += 1;
                _navMeshAgent.SetDestination(_points[_currentPatrollingPointIndex].position);
            }
        }

        public override void OnTriggerEnter(StraightForwardEnemy enemy, Collider collider)
        {
            _navMeshAgent.speed = 0;
            _animator.SetBool(Animations.Detecting, false);
            _animator.SetBool(Animations.Idle, true);
            _isEnd = true;
        }

        private void CheckForPlayer(StraightForwardEnemy straightForwardEnemy)
        {

            _detectionRadius?.Clear();
            straightForwardEnemy.ChangeState(straightForwardEnemy.ApproachingToAttackState);
        }

    }
}


