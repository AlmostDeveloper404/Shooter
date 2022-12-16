using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    [CreateAssetMenu(fileName = "Stats", menuName = "Enemy")]
    public class EnemyOverProgression : ScriptableObject
    {
        public int Damage;
        public int Health;
        public float FireRate;
    }
}


