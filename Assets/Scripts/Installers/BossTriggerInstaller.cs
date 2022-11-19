using UnityEngine;
using Zenject;

namespace Main
{
    public class BossTriggerInstaller : MonoInstaller
    {

        [SerializeField] private CutSceneActivator _bossTriggerActivator;
        public override void InstallBindings()
        {
            Container.Bind<CutSceneActivator>().FromInstance(_bossTriggerActivator).AsSingle();
        }
    }
}

