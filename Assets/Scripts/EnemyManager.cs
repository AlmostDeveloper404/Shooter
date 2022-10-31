using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private List<Enemy> _allLevelEnimies = new List<Enemy>();

        public static EnemyManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void RemoveEnemy(Enemy enemy)
        {
            _allLevelEnimies.Remove(enemy);
            if (_allLevelEnimies.Count == 0)
            {
                GameManager.ChangeGameState(GameState.LevelCompleted);
            }
        }

    }
}


