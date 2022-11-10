using System;
using UnityEngine;

namespace Main
{
    public class SpawnLoot : MonoBehaviour
    {
        private EnemyHealth _enemyHealth;




        [Header("Loot")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _loot;
        [SerializeField] private int _amount;
        [SerializeField] private float _angle;
        [SerializeField] private float _minSplashRadius;
        [SerializeField] private float _maxSplashRadius;


        private void Awake()
        {
            _enemyHealth = GetComponent<EnemyHealth>();
        }

        private void OnEnable()
        {
            _enemyHealth.OnDeath += Spawn;
        }
        private void OnDisable()
        {
            _enemyHealth.OnDeath -= Spawn;
        }


        private void Spawn()
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

