using UnityEngine;
using System;

namespace Main
{
    [Serializable]
    public struct Currency
    {
        public int Money;
    }

    public class PlayerResources : MonoBehaviour
    {
        private int _money;
        public int MoneyAmount { get { return _money; } }

        private int _keys;
        public int KeysAmount { get { return _keys; } }

        public Action<int> OnMoneyAmountChanged;
        public Action<int> OnKeysAmountChanged;

        private void Start()
        {
            Load();
        }

        private void OnEnable()
        {
            GameManager.OnLevelCompleted += Save;
        }

        private void OnDisable()
        {
            GameManager.OnLevelCompleted -= Save;
        }

        public void AddMoney(int amount)
        {
            _money += amount;
            OnMoneyAmountChanged?.Invoke(_money);
        }

        public void AddKey(int amount)
        {
            _keys += amount;
            OnKeysAmountChanged?.Invoke(_keys);
        }

        public void RemoveKey(int amount)
        {
            _keys -= amount;
            OnKeysAmountChanged?.Invoke(_keys);
        }

        public void RemoveMoney(int amount)
        {
            _money -= amount;
            OnMoneyAmountChanged?.Invoke(_money);
        }

        private void Save()
        {
            Currency currency = new Currency { Money = _money };
            SaveLoadProgress.SaveData(currency, UniqSavingId.Currency);
        }

        private void Load()
        {
            Currency currency = SaveLoadProgress.LoadData<Currency>(UniqSavingId.Currency);
            if (currency.Equals(default(Currency)))
            {
                _money = 0;
                OnMoneyAmountChanged?.Invoke(_money);
            }
            else
            {
                _money = currency.Money;
                OnMoneyAmountChanged?.Invoke(_money);
            }
        }

        [ContextMenu("DeleteData")]
        private void DeleteData()
        {
            SaveLoadProgress.DeleteData(UniqSavingId.Currency);
        }
    }
}

