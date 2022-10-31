using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Main
{
    public class PlayerInteractions : MonoBehaviour
    {
        [SerializeField] private Collider _playerInteractionCollider;

        private CompositeDisposable _onTriggerEnterDis = new CompositeDisposable();



        private void Start()
        {
            _playerInteractionCollider.OnTriggerEnterAsObservable().Where(t => t.gameObject.GetComponent<IInteractable>() as MonoBehaviour).Subscribe(_ => Interact(_.GetComponent<IInteractable>())).AddTo(_onTriggerEnterDis);
        }

        private void Interact(IInteractable interactable)
        {
            Debug.Log("Yep");
            interactable.Interact();
        }
        private void OnDestroy()
        {
            _onTriggerEnterDis?.Clear();
        }
    }
}


