using UnityEngine;
using Zenject;

namespace Main
{
    public class SoundsInstaller : MonoInstaller
    {
        [SerializeField] private Sounds _sounds;

        public override void InstallBindings()
        {
            Container.Bind<Sounds>().FromInstance(_sounds).AsSingle().NonLazy();
        }
    }
}
