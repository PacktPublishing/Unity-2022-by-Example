using System.Collections;
using UnityEngine;

public class ImpactApplyForce : MonoBehaviour
{
    [SerializeField]
    private float _impulseStrength = 3f;

    private Rigidbody _rb;
    private bool _forceEnabled = false;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        _forceEnabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_rb == null || !_forceEnabled)
            return;

        Vector3 impulseForce = -collision.contacts[0].normal * _impulseStrength;
        _rb.AddForce(impulseForce, ForceMode.Impulse);
    }
}