using UnityEngine;
using Zenject;

namespace Main
{
    public class BossTriggerInstaller : MonoInstaller
    {

        [SerializeField] private BossTriggerActivator _bossTriggerActivator;
        public override void InstallBindings()
        {
            Container.Bind<BossTriggerActivator>().FromInstance(_bossTriggerActivator).AsSingle();
        }
    }
}

