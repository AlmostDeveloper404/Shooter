using UnityEngine;
using Zenject;

namespace Main
{
    public class BossAnimation : MonoBehaviour
    {
        private Sounds _sounds;
        [SerializeField] private AudioClip _roarSound;

        [Inject]
        private void Construct(Sounds sounds)
        {
            _sounds = sounds;
        }

        public void PlayRoarSound()
        {
            _sounds.PlaySound(_roarSound);
        }
    }
}


