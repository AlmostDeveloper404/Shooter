using UnityEngine;

namespace Main
{
    public abstract class ClonBaseState
    {
        public abstract void EntryState(PlayerClon playerClon);

        public abstract void UpdateState(PlayerClon playerClon);
    }

}


