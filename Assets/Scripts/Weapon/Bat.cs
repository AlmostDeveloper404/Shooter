using UnityEngine;

namespace Main
{
    public class Bat : Weapon
    {
        [SerializeField] private int _damageAmount;
        public override void Attack(Transform enemy)
        {
            ITakeDamage takeDamage = enemy.GetComponent<ITakeDamage>();
            takeDamage.TakeDamage(_damageAmount);
        }
    }
}


