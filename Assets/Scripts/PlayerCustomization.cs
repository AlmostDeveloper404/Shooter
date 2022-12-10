using System;
using UnityEngine;
using Zenject;

namespace Main
{
    [Serializable]
    public struct ArmorOverUpgrade
    {
        public SkinnedMeshRenderer[] _allArmor;
    }

    public class PlayerCustomization : MonoBehaviour
    {
        private PlayerUpgrade _playerUpgrade;

        [SerializeField] private ArmorOverUpgrade[] _armor;
        private int _currentArmorIndex = 0;

        [Inject]
        private void Construct(PlayerController playerController)
        {
            _playerUpgrade = playerController.GetComponent<PlayerUpgrade>();
        }

        private void OnEnable()
        {
            _playerUpgrade.OnHealthUpgraded += UpdateArmor;
        }


        private void OnDisable()
        {
            _playerUpgrade.OnHealthUpgraded -= UpdateArmor;
        }

        private void UpdateArmor(int health, int amountOfUpgrades)
        {
            if (amountOfUpgrades >= _armor.Length - 1)
            {
                TakeOffArmor(_armor[_currentArmorIndex]);
                PutOnArmor(_armor[_armor.Length - 1]);
                return;
            }

            TakeOffArmor(_armor[_currentArmorIndex]);

            _currentArmorIndex = amountOfUpgrades;

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

