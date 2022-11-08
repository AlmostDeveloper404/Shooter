using UnityEngine;
using Zenject;

namespace Main
{
    public class ClonCustomization : MonoBehaviour
    {
        private PlayerUpgrade _playerUpgrade;

        [SerializeField] private ArmorOverUpgrade[] _armor;
        private int _currentArmorIndex = 0;

        private PlayerCustomization _playerCustomization;

        [Inject]
        private void Construct(PlayerController playerController, PlayerCustomization playerCustomization)
        {
            _playerUpgrade = playerController.GetComponent<PlayerUpgrade>();
            _playerCustomization = playerCustomization;
        }

        private void OnEnable()
        {
            _playerUpgrade.OnHealthUpgraded += UpdateArmor;
        }


        private void OnDisable()
        {
            _playerUpgrade.OnHealthUpgraded -= UpdateArmor;
        }

        private void Start()
        {
            SetupArmor();
        }

        private void SetupArmor()
        {
            _currentArmorIndex = _playerCustomization.ArmorIndex;
            TakeOffArmor(_armor[0]);
            PutOnArmor(_armor[_currentArmorIndex]);
        }

        private void UpdateArmor(int health)
        {
            if (_currentArmorIndex == _armor.Length - 1) return;

            TakeOffArmor(_armor[_currentArmorIndex]);

            _currentArmorIndex++;

            PutOnArmor(_armor[_currentArmorIndex]);
        }

        private void TakeOffArmor(ArmorOverUpgrade armorOverUpgrade)
        {
            foreach (var item in armorOverUpgrade._allArmor)
            {
                item.enabled = false;
            }
        }

        private void PutOnArmor(ArmorOverUpgrade armorOverUpgrade)
        {
            foreach (var item in armorOverUpgrade._allArmor)
            {
                item.enabled = true;
            }
        }

    }
}

