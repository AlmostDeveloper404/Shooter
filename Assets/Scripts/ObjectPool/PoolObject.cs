using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    [RequireComponent(typeof(Rigidbody))]
    public class PoolObject : MonoBehaviour, IPoolable<PoolObject>
    {
        private Action<PoolObject> _returnToPull;

        private void OnDisable()
        {
            ReturnToPool();
        }

        public void Initialize(Action<PoolObject> returnAction)
        {
            _returnToPull = returnAction;
        }

        public void ReturnToPool()
        {
            _returnToPull?.Invoke(this);
        }


    }
}


