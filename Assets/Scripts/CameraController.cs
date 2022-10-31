using UnityEngine;
using Zenject;

namespace Main
{
    public class CameraController : MonoBehaviour
    {
        private PlayerController _playerController;
        [SerializeField] private float _lerpSpeed;

        [Inject]
        private void Construct(PlayerController playerController)
        {
            _playerController = playerController;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _playerController.transform.position, _lerpSpeed * Time.deltaTime);
            
        }
    }
}


