using UnityEngine;
using System;
using UnityEngine.Playables;
using UniRx;
using UniRx.Triggers;

namespace Main
{
    public class CutSceneActivator : MonoBehaviour
    {
        public event Action OnBossFight;
        public event Action OnCutSceneEnded;
        [SerializeField] private PlayableDirector _playableDirector;

        [SerializeField] private GameObject[] _objectsToActivate;

        [SerializeField] private Animator _bossAnimator;


        private void Start()
        {
            StartCutScene();
        }

        public void StartCutScene()
        {
            _bossAnimator.SetTrigger(Animations.Roar);
            foreach (var item in _objectsToActivate)
            {
                item.SetActive(true);
            }
            _playableDirector.Play();
            OnBossFight?.Invoke();
        }



        public void ContinueGame()
        {
            foreach (var item in _objectsToActivate)
            {
                item.SetActive(false);
            }
            OnCutSceneEnded?.Invoke();
        }
    }
}


