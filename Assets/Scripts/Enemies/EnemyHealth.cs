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



        [Header("Loot")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _loot;
        [SerializeField] private int _amount;
        [SerializeField] private float _angle;
        [SerializeField] private float _minSplashRadius;
        [SerializeField] private float _maxSplashRadius;

        public event Action OnDeath;

        private Enemy _enemy;


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

            _healthBar.gameObject.SetActive(false);
            SpawnLoot();
            _targetRoom?.RemoveEnemy(_enemy);
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

