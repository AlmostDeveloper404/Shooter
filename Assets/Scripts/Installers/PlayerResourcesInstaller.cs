using UnityEngine;
using Zenject;

namespace Main
{
    public class PlayerResourcesInstaller : MonoInstaller
    {
        [SerializeField] private PlayerResources _playerResources;
        public override void InstallBindings()
        {
            Container.Bind<PlayerResources>().FromInstance(_playerResources).AsSingle();
        }
    }
}
