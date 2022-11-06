using UnityEngine;

namespace Main
{
    public static class CoinsSpawner
    {
        private static ObjectPool<Coin> _coinsPool;
        private static ObjectPool<CoinGrx> _coinGrx;

        private static bool _isFirstLaunch = true;
        private static bool _isFirstLaunchForGrx = true;
        public static Coin GetCoin(GameObject pref, Vector3 spawnPoint)
        {
            if (_isFirstLaunch)
            {
                _coinsPool = new ObjectPool<Coin>(pref);
                _isFirstLaunch = false;
            }

            return _coinsPool.Pull(spawnPoint);
        }

        public static CoinGrx GetCoinForGrx(GameObject pref, Vector3 spawnPoint)
        {
            if (_isFirstLaunchForGrx)
            {
                _coinGrx = new ObjectPool<CoinGrx>(pref);
                _isFirstLaunchForGrx = false;
            }

            return _coinGrx.Pull(spawnPoint);
        }
    }
}

