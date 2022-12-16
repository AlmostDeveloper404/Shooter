using UnityEngine;
using UnityEngine.AI;
using UniRx.Triggers;
using UniRx;

namespace Main
{
    public class StraightForwardPatrollingState : EnemyBaseState<StraightForwardEnemy>
    {
        private Vector3[] _points;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        private int _currentPatrollingPointIndex;

        private float _timer;
        private float _patrollingSpeed;
        private float _timeToStay;

        private float _runSpeed;

        private CompositeDisposable _onDetectionTriggerStay = new CompositeDisposable();

        private Collider _detectionCollider;
        private Collider _attackRadiusCollider;
        private LayerMask _layerMask;

        private bool _isDetection;
        private float _waitingAfterLosingTarget;

        private Weapon _weapon;

        private StraightForwardApproachingState _approachingState;

        public StraightForwardPatrollingState(Transform container, NavMeshAgent navMeshAgent, Animator animator, float timeToStay, Collider detectionRadiusCollider, Collider attackRadiusCollider, float runSpeed, float partolSpeed, float waitingAfterLosingTarget, Weapon weapon, LayerMask layerMask)
        {
            
            SetupPartollPoints(container);

            _layerMask = layerMask;
            _detectionCollider = detectionRadiusCollider;
            _timeToStay = timeToStay;
            _animator = animator;
            _navMeshAgent = navMeshAgent;
            _patrollingSpeed = partolSpeed;
            _runSpeed = runSpeed;
            _attackRadiusCollider = attackRadiusCollider;
            _weapon = weapon;
            _waitingAfterLosingTarget = waitingAfterLosingTarget;
        }


        private void SetupPartollPoints(Transform container)
        {
            _points = new Vector3[container.childCount];
            for (int i = 0; i < _points.Length; i++)
            {
                _points[i] = container.GetChild(i).position;
            }
        }


        public override void EntryState(StraightForwardEnemy straightForwardEnemy)
        {

            _animator.SetBool(Animations.Idle, true);
            _animator.SetBool(Animations.Detecting, false);

            _navMeshAgent.speed = _patrollingSpeed;

            _onDetectionTriggerStay?.Clear();

            _detectionCollider.OnTriggerStayAsObservable().Where(t => t.gameObject.GetComponent<Unit>()).Subscribe(_ => TryAttack(straightForwardEnemy, _)).AddTo(_onDetectionTriggerStay);

            _currentPatrollingPointIndex = 0;
            _navMeshAgent.SetDestination(_points[_currentPatrollingPointIndex]);

        }
        public override void UpdateState(StraightForwardEnemy straightForwardEnemy)
        {
            #region PatrollingLogic
            if (_navMeshAgent.velocity == Vector3.zero)
            {
                if (_isDetection)
                {
                    _animator.SetBool(Animations.Idle, true);
                    _animator.SetBool(Animations.Detecting, false);
                    _isDetection = false;
                }


                _timer += Time.deltaTime;
                if (_timer > _timeToStay)
                {
                    _timer = 0;
                    _currentPatrollingPointIndex = _currentPatrollingPointIndex == _points.Length - 1 ? 0 : _currentPatrollingPointIndex += 1;
                    _navMeshAgent.SetDestination(_points[_currentPatrollingPointIndex]);
                }
            }
            else
            {
                if (!_isDetection)
                {
                    _animator.SetBool(Animations.Idle, false);
                    _animator.SetBool(Animations.Detecting, true);
                    _isDetection = true;
                }

            }
            #endregion
        }

        private void TryAttack(StraightForwardEnemy straightForwardEnemy, Collider collider)
        {
            bool result = HasDirectView<Unit>.HasView(straightForwardEnemy.transform.position, collider.transform.position, _layerMask);
            if (result)
            {
                _onDetectionTriggerStay?.Clear();
                _navMeshAgent.SetDestination(collider.transform.position);
                _approachingState = new StraightForwardApproachingState(_navMeshAgent, _animator, _runSpeed, _waitingAfterLosingTarget, _attackRadiusCollider, _detectionCollider, _weapon, _layerMask, this);
                straightForwardEnemy.ChangeState(_approachingState);
            }
        }

        public override void ExitState(StraightForwardEnemy straightForwardEnemy)
        {
            _onDetectionTriggerStay?.Clear();
        }
    }
}


