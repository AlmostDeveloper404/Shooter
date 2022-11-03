using UnityEngine;

namespace Main
{
    public static class CoinsSpawner
    {
        private static ObjectPool<Coin> _coinsPool;

        private static bool _isFirstLaunch=true;
        public static Coin GetCoin(GameObject pref,Vector3 spawnPoint)
        {
            if (_isFirstLaunch)
            {
                _coinsPool = new ObjectPool<Coin>(pref);
                _isFirstLaunch = false;
            }

            return _coinsPool.Pull(spawnPoint);
            
        }
    }
}

