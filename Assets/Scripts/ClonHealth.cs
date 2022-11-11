using System;
using UnityEngine;
using Zenject;

namespace Main
{
    public class ClonHealth : MonoBehaviour, ITakeDamage
    {
        private int _currentHealth;

        private HealthBar _healthBar;
        private Rigidbody _rigidbody;
        private Animator _animator;
        private PlayerUpgrade _playerUpgrade;
        private PlayerHealth _playerHealth;

        private int _maxHealth;

        [Inject]
        private void Construct(PlayerController playerController)
        {
            _playerUpgrade = playerController.GetComponent<PlayerUpgrade>();
            _playerHealth = playerController.GetComponent<PlayerHealth>();
        }

        private void Awake()
        {
            _healthBar = GetComponentInChildren<HealthBar>();
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();

        }

        private void OnEnable()
        {
            _playerUpgrade.OnHealthUpgraded += HealthUpgraded;
            _playerUpgrade.OnUpgraded += Heal;
        }


        private void OnDisable()
        {
            _playerUpgrade.OnHealthUpgraded -= HealthUpgraded;
            _playerUpgrade.OnUpgraded -= Heal;
        }


        private void Start()
        {
            UpdateHealth();
        }

        private void Heal()
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
            _maxHealth = _playerHealth.MaxHealth;
            _currentHealth = _maxHealth;

            _healthBar.UpdateUI(_maxHealth, _currentHealth);
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            _healthBar.UpdateUI(_maxHealth, _currentHealth);
            if (_currentHealth <= 0)
            {
                Death();
            }
        }


        private void Death()
        {
            GetComponent<CapsuleCollider>().enabled = false;
            _healthBar.gameObject.SetActive(false);
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _animator.applyRootMotion = true;
            _animator.SetTrigger(Animations.Death);
            enabled = false;
        }
    }
}

