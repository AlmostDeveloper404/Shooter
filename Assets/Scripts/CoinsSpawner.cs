using UnityEngine;
using Zenject;

namespace Main
{
    public class CoinsSpawner : MonoBehaviour
    {
        private ObjectPool<Coin> _coinsPool;
        private ObjectPool<CoinGrx> _coinGrx;

        [SerializeField] private GameObject _coin;
        [SerializeField] private GameObject _coinGRX;

        private Sounds _sounds;
        private DiContainer _diContainer;

        [Inject]
        private void Construct(Sounds sounds, DiContainer diContainer)
        {
            _sounds = sounds;
            _diContainer = diContainer;
        }


        private void Start()
        {
            _coinsPool = new ObjectPool<Coin>(_coin, _diContainer);
            _coinGrx = new ObjectPool<CoinGrx>(_coinGRX);
        }

        public Coin GetCoin(Vector3 spawnPoint)
        {
            return _coinsPool.PullZenject(spawnPoint, Quaternion.identity);
        }

        public CoinGrx GetCoinForGrx(Vector3 spawnPoint)
        {
            return _coinGrx.Pull(spawnPoint);
        }
    }
}

