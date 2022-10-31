using System;
using UnityEngine;

namespace Main
{
    public class Key : MonoBehaviour, IInteractable, IPoolable<Key>
    {
        [SerializeField] private int _keysAmount;

        private Action<Key> _returnAction;

        private void OnDisable()
        {
            ReturnToPool();
        }

        public void Initialize(Action<Key> returnAction)
        {
            _returnAction = returnAction;
        }

        public void Interact()
        {
            PlayerResources.AddKey(_keysAmount);
            gameObject.SetActive(false);
        }

        public void ReturnToPool()
        {
            _returnAction?.Invoke(this);
        }
    }
}



