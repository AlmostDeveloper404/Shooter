using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Main
{
    public class PlayerInteractions : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider _playerInteractionCollider;
        private CompositeDisposable _onTriggerEnterDis = new CompositeDisposable();


        private void OnEnable()
        {
            _playerInteractionCollider.OnTriggerEnterAsObservable().Where(t => t.gameObject.GetComponent<IInteractable>() as MonoBehaviour).Subscribe(_ => Interact(_.GetComponent<IInteractable>())).AddTo(_onTriggerEnterDis);
        }

        private void Interact(IInteractable interactable)
        {
            interactable.Interact();
        }

        private void OnDisable()
        {
            _onTriggerEnterDis?.Clear();
        }
    }
}


