using UnityEngine;

namespace Main
{
    public class CoinsSpawner : MonoBehaviour
    {
        private ObjectPool<Coin> _coinsPool;
        private ObjectPool<CoinGrx> _coinGrx;

        [SerializeField] private GameObject _coin;
        [SerializeField] private GameObject _coinGRX;

        private void Start()
        {
            _coinsPool = new ObjectPool<Coin>(_coin);
            _coinGrx = new ObjectPool<CoinGrx>(_coinGRX);
        }

        public Coin GetCoin(Vector3 spawnPoint)
        {
            return _coinsPool.Pull(spawnPoint);
        }

        public CoinGrx GetCoinForGrx(Vector3 spawnPoint)
        {
            return _coinGrx.Pull(spawnPoint);
        }
    }
}

