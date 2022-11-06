using UnityEngine;

namespace Main
{
    public class Enemy : MonoBehaviour, ITakeDamage
    {
        public bool IsDead { get; set; }

        

        public virtual void TakeDamage(int damage)
        {

        }
    }
}


