using UnityEngine;

namespace Main
{
    public static class Animations
    {
        public static int Idle = Animator.StringToHash("Idle");
        public static int Run = Animator.StringToHash("Run");
        public static int Detecting = Animator.StringToHash("Detecting");
        public static int Attack = Animator.StringToHash("Attack");
        public static int Death = Animator.StringToHash("Death");
        public static int Roar = Animator.StringToHash("Roar");
        public static int UpgradeFirst = Animator.StringToHash("UpgradeFirst");
        public static int UpgradeSecond = Animator.StringToHash("UpgradeSecond");
    }
}


