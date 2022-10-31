using UnityEngine;

namespace Main
{
    public class Weapon : MonoBehaviour, IDoDamage
    {
        public virtual void Attack(Transform enemy)
        {

        }
    }
}


