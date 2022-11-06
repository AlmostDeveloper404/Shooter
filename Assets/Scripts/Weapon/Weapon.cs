using UnityEngine;
using Zenject;

namespace Main
{
    public class Weapon : MonoBehaviour, IDoDamage
    {
        private PlayerController _playerController;

        [SerializeField] private int _damage;

        public int Damage { get { return _damage; } }

        [Inject]
        private void Construct(PlayerController playerController)
        {
            _playerController = playerController;
        }

        private void OnEnable()
        {
            _playerController.OnDamageChanged += UpgradeDamage;
        }

        private void OnDisable()
        {
            _playerController.OnDamageChanged -= UpgradeDamage;
        }

        private void UpgradeDamage(int amount)
        {
            _damage += amount;
            _playerController.UpdateBehaviour();
        }

        public virtual void Attack(Transform enemy)
        {

        }
    }
}


