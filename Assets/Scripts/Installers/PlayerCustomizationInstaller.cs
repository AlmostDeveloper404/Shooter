using UnityEngine;
using Zenject;

namespace Main
{
    public class PlayerCustomizationInstaller : MonoInstaller
    {
        [SerializeField] private PlayerCustomization _playerCustomization;

        public override void InstallBindings()
        {
            Container.Bind<PlayerCustomization>().FromInstance(_playerCustomization).AsSingle();
        }
    }
}
