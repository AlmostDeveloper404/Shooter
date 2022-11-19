using UnityEngine;

namespace Main
{
    public abstract class PlayerBaseState
    {
        public abstract void EntryState(PlayerController playerController);
        public abstract void Update(PlayerController playerController);
        public abstract void FixedUpdate(PlayerController playerController);
    }
}


