using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Main
{
    public class StraightForwardEnemy : Enemy
    {
        private EnemyBaseState<StraightForwardEnemy> _currentState;
        private StraightForwardPatrollingState _patrolling;
        private StraightForwardApproachingState _approaching;
        private StraightForwardAttackState _attack;

        public StraightForwardApproachingState ApproachingToAttackState { get { return _approaching; } }
        public StraightForwardAttackState StraightForwardAttackState { get { return _attack; } }

        [SerializeField] private float _timeToStayNearbyPatrollingPoint;

        private NavMeshAgent _navMeshAgent;

        [Header("AdjustableModules")]
        [SerializeField] private Weapon _enemyWeapon;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _patrolPointsContainer;
        [SerializeField] private Collider _detectionRadius;
        [SerializeField] private Collider _attackRadius;
        [SerializeField] private float _patrollingSpeed;
        [SerializeField] private float _runningSpeed;
        [SerializeField] private int _health;
        [SerializeField] private float _attackRate;
        [SerializeField] private LayerMask _rayMask;


        private PlayerController _playerController;


        [Inject]
        private void Construct(PlayerController playerController)
        {
            _playerController = playerController;
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += GameOver;
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= GameOver;
        }


        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }


        private void Start()
        {
            _patrolPointsContainer.transform.parent = null;
            _patrolling = new StraightForwardPatrollingState(_patrolPointsContainer, _navMeshAgent, _animator, _timeToStayNearbyPatrollingPoint, _detectionRadius, _patrollingSpeed);
            _approaching = new StraightForwardApproachingState(_playerController, _navMeshAgent, _animator, _runningSpeed, _attackRadius, _rayMask);
            _attack = new StraightForwardAttackState(_playerController, _animator, _attackRadius, _navMeshAgent, _attackRate, _enemyWeapon);
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

        public override void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                IsDead = true;
                EnemyManager.Instance.RemoveEnemy(this);
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            _currentState?.OnTriggerEnter(this, other);
        }
        private void GameOver()
        {
            _navMeshAgent.speed = 0;
            _currentState = null;
            _animator.SetTrigger(Animations.Idle);
        }
    }
}

