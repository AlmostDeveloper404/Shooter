using UnityEngine;
using Zenject;

namespace Main
{
    public static class ClonCreator
    {
        private static ObjectPool<PlayerClon> _clonsPull;
        private static bool _isFirstLaunch = true;

        private static DiContainer _diContainer;

        public static PlayerClon CreateClon(Vector3 spawnPosition, GameObject clonGO, DiContainer diContainer)
        {
            if (_isFirstLaunch)
            {
                _diContainer = diContainer;
                _clonsPull = new ObjectPool<PlayerClon>(clonGO, _diContainer);
                _isFirstLaunch = false;
            }

            PlayerClon playerClon = _clonsPull.PullZenject();
            playerClon.transform.position = spawnPosition;
            return playerClon;
        }
    }
}

