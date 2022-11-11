using UnityEngine;

namespace Main
{
    public static class CulculateVelocity
    {
        private static float g = Physics.gravity.y;
        public static Vector3 Culculate(Vector3 objectPosition, Vector3 target, Vector3 launchDirection, float angleInDegree)
        {

            Vector3 fromTo = target - objectPosition;
            Vector3 fromToXZ = new Vector3(fromTo.x, 0f, fromTo.z);

            float x = fromToXZ.magnitude;
            float y = fromTo.y;

            float _angleInRadians = angleInDegree * Mathf.PI / 180f;

            float v2 = (g * x * x) / (2 * (y - Mathf.Tan(_angleInRadians) * x) * Mathf.Pow(Mathf.Cos(_angleInRadians), 2));
            float v = Mathf.Sqrt(Mathf.Abs(v2));

            return launchDirection.normalized * v;
        }
    }
}

