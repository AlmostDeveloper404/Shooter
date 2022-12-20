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
        private PlayerResources _playerResources;

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        [Inject]
        private void Construct(Sounds sounds, PlayerResources playerResources)
        {
            _sounds = sounds;
            _playerResources = playerResources;
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
            _collider.isTrigger = false;
            _playerResources.AddKey(_keysAmount);
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



