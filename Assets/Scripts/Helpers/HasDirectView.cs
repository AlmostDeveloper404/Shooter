using UnityEngine;

namespace Main
{
    public static class HasDirectView<T> where T : MonoBehaviour
    {

        public static bool HasView(Vector3 aPosition, Vector3 bPosition, LayerMask layerMask)
        {
            Vector3 direction = bPosition - aPosition;
            Ray ray = new Ray(aPosition + Vector3.up, direction);

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, direction.magnitude + 1, layerMask))
            {
                return hitInfo.collider.GetComponent<T>() ? true : false;
            }
            return false;
        }

    }
}


