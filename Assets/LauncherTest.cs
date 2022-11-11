using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherTest : MonoBehaviour
{
    [SerializeField] private GameObject _rocket;
    [SerializeField] private Transform TargetTransform;

    [SerializeField] private Transform SpawnTransform;

    [SerializeField] private float _angleInDegrees;
    private float g = Physics.gravity.y;

    private void Update()
    {
        SpawnTransform.localEulerAngles = new Vector3(-_angleInDegrees, 0f, 0f);
        if (Input.GetMouseButtonDown(0))
        {
            Launch();
        }
    }

    [ContextMenu("Attack")]
    private void Launch()
    {
        Vector3 fromTo = TargetTransform.position - transform.position;
        Vector3 fromToXZ = new Vector3(fromTo.x, 0f, fromTo.z);

        
        transform.rotation = Quaternion.LookRotation(fromToXZ, Vector3.up);

        float x = fromToXZ.magnitude;
        float y = fromTo.y;

        float _angleInRadians = _angleInDegrees * Mathf.PI / 180f;

        float v2 = (g * x * x) / (2 * (y - Mathf.Tan(_angleInRadians) * x) * Mathf.Pow(Mathf.Cos(_angleInRadians), 2));
        float v = Mathf.Sqrt(Mathf.Abs(v2));

        GameObject rocket = Instantiate(_rocket, SpawnTransform.position, Quaternion.identity);
        rocket.GetComponent<Rigidbody>().velocity = SpawnTransform.forward * v;
    }
}
