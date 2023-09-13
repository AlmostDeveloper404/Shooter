using UnityEngine;
using Zenject;

namespace Main
{
    public class CameraController : MonoBehaviour
    {
        private PlayerController _playerController;
        [SerializeField] private float _lerpSpeed;

        [SerializeField] private float _cameraOffset;

        [Inject]
        private void Construct(PlayerController playerController)
        {
            _playerController = playerController;
        }

        private void LateUpdate()
        {
            Vector3 direction = _playerController.transform.forward.normalized;
            direction.y = 0;

            transform.position = Vector3.Lerp(transform.position, _playerController.transform.position + direction * _cameraOffset, _lerpSpeed * Time.deltaTime);
        }
    }
}


