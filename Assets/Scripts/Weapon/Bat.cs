using UnityEngine;

namespace Main
{
    public class Bat : Weapon
    {
        public override void Attack(Transform enemy)
        {
            ITakeDamage takeDamage = enemy.GetComponent<ITakeDamage>();
            takeDamage.TakeDamage(Damage);
        }
    }
}


