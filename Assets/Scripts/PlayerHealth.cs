using UnityEngine;

namespace Main
{
    public class PlayerHealth : MonoBehaviour, ITakeDamage
    {

        private int _currentHealth;

        private HealthBar _healthBar;
        private Rigidbody _rigidbody;
        private Animator _animator;
        private PlayerUpgrade _playerUpgrade;

        [SerializeField] private int _maxHealth;

        public int MaxHealth { get { return _maxHealth; } }

        private void Awake()
        {
            _healthBar = GetComponentInChildren<HealthBar>();
            _rigidbody = GetComponent<Rigidbody>();
            _playerUpgrade = GetComponent<PlayerUpgrade>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            _playerUpgrade.OnHealthUpgraded += HealthUpgraded;
        }

        private void OnDisable()
        {
            _playerUpgrade.OnHealthUpgraded -= HealthUpgraded;
        }

        private void Start()
        {
            _currentHealth = _maxHealth;
            _healthBar.UpdateUI(_maxHealth, _currentHealth);
        }

        private void HealthUpgraded(int amount)
        {
            _maxHealth += amount;
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            _currentHealth = _maxHealth;

            _healthBar.UpdateUI(_maxHealth, _currentHealth);
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            _healthBar.UpdateUI(_maxHealth, _currentHealth);
            if (_currentHealth <= 0)
            {
                GameManager.ChangeGameState(GameState.GameOver);
                Death();
            }
        }


        private void Death()
        {
            _healthBar.gameObject.SetActive(false);
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _animator.applyRootMotion = true;
            _animator.SetTrigger(Animations.Death);
            enabled = false;
        }

    }
}

