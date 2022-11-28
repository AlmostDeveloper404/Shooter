using System;
using UnityEngine;
using System.Collections;
using Zenject;

namespace Main
{
    public class Key : MonoBehaviour, IInteractable, IPoolable<Key>
    {
        [SerializeField] private int _keysAmount;
        [SerializeField] private float _timeToDisappear;

        [SerializeField] private GameObject _collectParticles;
        [SerializeField] private GameObject _glowingParticles;

        private Action<Key> _returnAction;

        [SerializeField] private AudioClip _keysPickUp;

        private Sounds _sounds;

        [Inject]
        private void Construct(Sounds sounds)
        {
            _sounds = sounds;
        }

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
            StartCoroutine(DisableKey());
            _sounds.PlaySound(_keysPickUp);

        }

        public void ReturnToPool()
        {
            _returnAction?.Invoke(this);
        }

        private IEnumerator DisableKey()
        {
            _glowingParticles.SetActive(false);
            _collectParticles.SetActive(true);
            yield return Helpers.Helper.GetWait(_timeToDisappear);
            gameObject.SetActive(false);
        }
    }
}



