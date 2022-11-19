using UnityEngine;
using System;

namespace Main
{
    public class EnemyHealth : MonoBehaviour, ITakeDamage
    {

        private HealthBar _healthBar;


        private int _currentHealth;
        [SerializeField] private int _health;


        private Room _targetRoom;

        public event Action OnDeath;

        private Enemy _enemy;

        [SerializeField] private bool _isBoss = false;


        private void Awake()
        {
            _healthBar = GetComponentInChildren<HealthBar>();
            _targetRoom = GetComponentInParent<Room>();
            _enemy = GetComponent<Enemy>();
        }

        private void Start()
        {
            _currentHealth = _health;
            _healthBar.UpdateUI(_health, _currentHealth);
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            _healthBar.UpdateUI(_health, _currentHealth);
            if (_currentHealth <= 0)
            {
                Death();
            }
        }


        private void Death()
        {
            OnDeath?.Invoke();
            if (_isBoss)
            {
                GameManager.ChangeGameState(GameState.LevelCompleted);
            }

            _healthBar.gameObject.SetActive(false);
            _targetRoom?.RemoveEnemy(_enemy);
        }



    }
}

