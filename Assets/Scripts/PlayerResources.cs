using UnityEngine;
using System;

namespace Main
{
    public static class PlayerResources
    {
        private static int _money;
        public static int MoneyAmount { get { return _money; } }

        private static int _keys;
        public static int KeysAmount { get { return _keys; } }

        public static Action<int> OnMoneyAmountChanged;
        public static Action<int> OnKeysAmountChanged;

        public static void AddMoney(int amount)
        {
            _money += amount;
            OnMoneyAmountChanged?.Invoke(_money);
        }

        public static void AddKey(int amount)
        {
            _keys += amount;
            OnKeysAmountChanged?.Invoke(_keys);
        }

        public static void RemoveKey()
        {
            _keys -= 1;
            OnKeysAmountChanged?.Invoke(_keys);
        }

        public static void RemoveMoney(int amount)
        {
            _money -= amount;
            OnMoneyAmountChanged?.Invoke(_money);
        }
    }
}

