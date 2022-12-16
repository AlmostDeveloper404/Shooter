using UnityEngine;
using Zenject;

namespace Main
{
    public class BossAnimation : MonoBehaviour
    {
        private Sounds _sounds;
        [SerializeField] private AudioClip _roarSound;

        private Animator _animator;

        [Inject]
        private void Construct(Sounds sounds)
        {
            _sounds = sounds;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _animator.SetTrigger(Animations.Roar);
        }

        public void PlayRoarSound()
        {
            _sounds.PlaySound(_roarSound);
        }
    }
}


