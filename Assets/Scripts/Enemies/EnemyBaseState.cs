using UnityEngine;

namespace Main
{
    public abstract class EnemyBaseState<T> where T : Enemy
    {
        public abstract void EntryState(T straightForwardEnemy);

        public abstract void UpdateState(T straightForwardEnemy);

        public abstract void OnTriggerEnter(T enemy, Collider collider);


    }
}


