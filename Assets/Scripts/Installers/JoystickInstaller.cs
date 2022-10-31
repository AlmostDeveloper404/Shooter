using UnityEngine;
using Zenject;

namespace Main 
{
    public class JoystickInstaller : MonoInstaller
    {
        [SerializeField] private FloatingJoystick _fixedJoystick;

        public override void InstallBindings()
        {
            Container.Bind<FloatingJoystick>().FromInstance(_fixedJoystick);
        }
    }
}

