using UnityEngine;

namespace Main
{
    public static class CulculateVelocity
    {
        public static Vector3 Culculate(Vector3 objectPosition, Vector3 target, Vector3 launchDirection, float angleInDegree)
        {
            float _angleInRadians = angleInDegree * Mathf.PI / 180;

            Vector3 direction = target - objectPosition;
            Vector3 fromToXZ = new Vector3(direction.x, 0f, direction.z);

            float x = fromToXZ.magnitude;
            float y = direction.y;

            float v2 = (Physics.gravity.y * x * x) * 0.5f * (y - Mathf.Tan(_angleInRadians) * x) * Mathf.Pow(Mathf.Cos(_angleInRadians), 2);
            float v = Mathf.Sqrt(Mathf.Abs(v2));

            return launchDirection * v;
        }
    }
}

