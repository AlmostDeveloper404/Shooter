using UnityEngine;
using System;

namespace Main
{
    public class LootDrop : MonoBehaviour,IPoolable<LootDrop>
    {
        private Action<LootDrop> _returnAction;

        public void Initialize(Action<LootDrop> returnAction)
        {
            _returnAction = returnAction;
        }

        public void ReturnToPool()
        {
            _returnAction?.Invoke(this);
        }
    }
}

