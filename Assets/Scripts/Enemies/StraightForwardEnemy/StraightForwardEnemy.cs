using UnityEngine;
using UnityEngine.AI;

namespace Main
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class StraightForwardEnemy : Enemy
    {
        private EnemyBaseState<StraightForwardEnemy> _currentState;
        private StraightForwardPatrollingState _patrolling;

        [SerializeField] private float _timeToStayNearbyPatrollingPoint;

        private NavMeshAgent _navMeshAgent;

        [Header("Enemy behavior")]
        [SerializeField] private Weapon _enemyWeapon;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _patrolPointsContainer;
        [SerializeField] private Collider _detectionRadius;
        [SerializeField] private Collider _attackRadius;
        [SerializeField] private float _patrollingSpeed;
        [SerializeField] private float _runningSpeed;
        [SerializeField] private float _waitingAfterLosingTarget;

        [SerializeField] private LayerMask _rayMask;

        private EnemyHealth _enemyHealth;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _enemyHealth = GetComponent<EnemyHealth>();
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += GameOver;
            _enemyHealth.OnDeath += Death;
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= GameOver;
            _enemyHealth.OnDeath -= Death;
        }

        private void Start()
        {

            _patrolPointsContainer.transform.parent = null;

            _patrolling = new StraightForwardPatrollingState(_patrolPointsContainer, _navMeshAgent, _animator, _timeToStayNearbyPatrollingPoint, _detectionRadius, _attackRadius, _runningSpeed, _patrollingSpeed, _waitingAfterLosingTarget, _enemyWeapon, _rayMask);
            _currentState = _patrolling;
            _currentState?.EntryState(this);
        }

        private void Update()
        {
            _currentState?.UpdateState(this);
        }
        public void ChangeState(EnemyBaseState<StraightForwardEnemy> state)
        {
            _currentState = state;
            _currentState?.EntryState(this);
        }

        private void GameOver()
        {
            _navMeshAgent.speed = 0;
            _currentState = null;
            _animator.SetBool(Animations.Idle, true);
            _animator.SetBool(Animations.Run, false);
            _animator.SetBool(Animations.Attack, false);
            _animator.SetBool(Animations.Detecting, false);
        }

        private void Death()
        {
            _currentState = null;
            _detectionRadius.enabled = false;
            _attackRadius.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            _navMeshAgent.velocity = Vector3.zero;
            _navMeshAgent.speed = 0;

            _animator.applyRootMotion = true;
            _animator.SetTrigger(Animations.Death);
            IsDead = true;
            _navMeshAgent.enabled = false;

        }

    }
}

