using UnityEngine;
using Zenject;

namespace Main
{
    public class BossAnimation : MonoBehaviour
    {
        private Sounds _sounds;
        [SerializeField] private AudioClip _roarSound;

        private Animator _animator;
        private CutSceneActivator _cutSceneActivator;

        [Inject]
        private void Construct(Sounds sounds,CutSceneActivator cutSceneActivator)
        {
            _sounds = sounds;
            _cutSceneActivator = cutSceneActivator;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _cutSceneActivator.StartCutScene();
            _animator.SetTrigger(Animations.Roar);
        }

        public void PlayRoarSound()
        {
            _sounds.PlaySound(_roarSound);
        }
    }
}


