using UnityEngine;

namespace Main
{
    public class Bat : Weapon
    {
        public override void Attack(Transform enemy)
        {
            base.Attack(enemy);

            ITakeDamage takeDamage = enemy.GetComponent<ITakeDamage>();
            takeDamage.TakeDamage(Damage);
        }
    }
}


