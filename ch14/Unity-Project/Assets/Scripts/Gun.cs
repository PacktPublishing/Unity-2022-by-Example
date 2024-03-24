using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour, IDamage
{
    [SerializeField] private float _range = 100f;
    [SerializeField] private Transform _firingPoint;
    public int DamageAmount => _damageAmount;
    [SerializeField] private int _damageAmount = 20;
    public LayerMask DamageMask => _damageMask;
    [SerializeField] private LayerMask _damageMask;

    private LineRenderer _lineRenderer;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;
    }

    public void Shoot()
    {
        Vector3 start = _firingPoint.position;
        Vector3 end = start + (_firingPoint.forward * _range);

        if (Physics.Raycast(start, _firingPoint.forward, out RaycastHit hit, _range, _damageMask))
        {
            end = hit.point;
        }

        StartCoroutine(ShowLaserBeam());
        IEnumerator ShowLaserBeam()
        {
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);

            yield return new WaitForSeconds(0.1f);
            _lineRenderer.enabled = false;
        }

        // Try to apply damage directly to the object that was hit, if it has a health system component (always on the root object of the Prefab).
        if (hit.transform.root.TryGetComponent<HealthSystem>(out var health))
        {
            health.HandleDamageCollision(null, this);
        }
    }

    public void DoDamage(Collider collision, bool isAffected)
    { }
}