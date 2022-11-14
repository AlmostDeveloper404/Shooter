using UnityEngine;
using System;
using UnityEngine.Playables;
using UniRx;
using UniRx.Triggers;

namespace Main
{
    public class BossTriggerActivator : MonoBehaviour
    {
        public event Action OnBossFight;
        public event Action OnCutSceneEnded;
        [SerializeField] private PlayableDirector _playableDirector;

        private CompositeDisposable _onTriggerEnterDis = new CompositeDisposable();

        private BoxCollider _boxCollider;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }

        private void Start()
        {
            //_boxCollider.OnTriggerEnterAsObservable().Where(t => t.GetComponent<PlayerController>()).Subscribe(_ => StartCutScene()).AddTo(_onTriggerEnterDis);
        }

        public void StartCutScene()
        {
            _playableDirector.Play();
            OnBossFight?.Invoke();
            //_onTriggerEnterDis?.Clear();
        }



        public void ContinueGame()
        {
            OnCutSceneEnded?.Invoke();
        }
    }
}


