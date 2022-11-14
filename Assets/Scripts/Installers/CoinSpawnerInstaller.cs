using UnityEngine;
using Zenject;

namespace Main
{
    public class CoinSpawnerInstaller : MonoInstaller
    {
        [SerializeField] private CoinsSpawner _coinSpawner;

        public override void InstallBindings()
        {
            Container.Bind<CoinsSpawner>().FromInstance(_coinSpawner).AsSingle();
        }
    }
}

