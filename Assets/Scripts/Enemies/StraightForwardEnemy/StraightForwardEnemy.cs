using UnityEngine;
using UnityEngine.AI;
using System.Collections;
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

        [Header("Enemy behavior")]
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

        [Header("Loot")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _loot;
        [SerializeField] private int _amount;
        [SerializeField] private float _angle;
        [SerializeField] private float _minSplashRadius;
        [SerializeField] private float _maxSplashRadius;

        private PlayerController _playerController;
        private Room _targetRoom;


        [Inject]
        private void Construct(PlayerController playerController)
        {
            _playerController = playerController;
        }
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _targetRoom = GetComponentInParent<Room>();
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += GameOver;
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= GameOver;
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
                Death();
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

        private void Death()
        {
            SpawnLoot();
            IsDead = true;
            _targetRoom?.RemoveEnemy(this);
            _currentState = null;
            gameObject.SetActive(false);
        }

        private void SpawnLoot()
        {
            float currentYAngle = 0;
            float angleInterval = 360f / _amount;

            Vector3 point = Vector3.zero;


            for (int i = 0; i < _amount; i++)
            {
                Coin coin = CoinsSpawner.GetCoin(_loot, _spawnPoint.position);
                _spawnPoint.localRotation = Quaternion.Euler(0f, currentYAngle, 0f);


                point = transform.position + _spawnPoint.forward * UnityEngine.Random.Range(_minSplashRadius, _maxSplashRadius);

                currentYAngle += angleInterval;
                coin.Launch(point, _spawnPoint.forward, _angle);
            }
        }

    }
}

