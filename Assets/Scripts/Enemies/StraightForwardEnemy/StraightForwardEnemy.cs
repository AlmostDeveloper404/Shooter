using UnityEngine;
using UnityEngine.AI;
using Zenject;
using UnityEngine.SceneManagement;
using System;

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
        [SerializeField] private ParticleSystem _deathParticles;
        [SerializeField] private Vector3 _particlesOffset;

        [SerializeField] private AudioClip _deathSound;

        private Sounds _sounds;

        [SerializeField] private EnemyTypes _enemyType;

        private EnemyOverProgression _enemyOverProgression;

        [Inject]
        private void Construct(Sounds sounds)
        {
            _sounds = sounds;
        }


        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _enemyHealth = GetComponent<EnemyHealth>();

            SetupEnemiesOverProgression();
        }

        private void SetupEnemiesOverProgression()
        {
            int levelIndex = SceneManager.GetActiveScene().buildIndex;
            _enemyOverProgression = Resources.Load<EnemyOverProgression>($"Stats/Level{levelIndex}/{_enemyType}/Stats");
            

            _patrolling = new StraightForwardPatrollingState(_patrolPointsContainer, _navMeshAgent, _animator, _timeToStayNearbyPatrollingPoint, _detectionRadius, _attackRadius, _runningSpeed, _patrollingSpeed, _waitingAfterLosingTarget, _enemyWeapon, _rayMask);
        }

        private void OnEnable()
        {
            _currentState = _patrolling;
            _currentState?.EntryState(this);

            GameManager.OnGameOver += GameOver;
            _enemyHealth.OnDeath += Death;
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= GameOver;
            _enemyHealth.OnDeath -= Death;


            _currentState?.ExitState(this);
            _currentState = null;
        }

        private void Start()
        {
            _deathParticles.transform.parent = null;
            _patrolPointsContainer.transform.parent = null;

            _enemyWeapon.SetupWeapon(_enemyOverProgression);
            _enemyHealth.SetupHealth(_enemyOverProgression);
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
            _sounds.PlaySound(_deathSound);
            _deathParticles.transform.position = transform.position + _particlesOffset;
            _deathParticles.Play();
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

