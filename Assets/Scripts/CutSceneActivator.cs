using UnityEngine;
using System;
using UnityEngine.Playables;

namespace Main
{
    public class CutSceneActivator : MonoBehaviour
    {
        public event Action OnBossFight;
        public event Action OnCutSceneEnded;
        [SerializeField] private PlayableDirector _playableDirector;

        [SerializeField] private GameObject[] _objectsToActivate;

        
        public void StartCutScene()
        {
            Debug.Log("Yep");
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


