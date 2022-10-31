using UnityEngine;
using Zenject;

namespace Main
{
    public class PlayerControllerInstaller : MonoInstaller
    {
        [SerializeField] private PlayerController _playerController;


        public override void InstallBindings()
        {
            Container.Bind<PlayerController>().FromInstance(_playerController);
        }
    }
}
