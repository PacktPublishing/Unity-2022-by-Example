using UnityEngine;

public class WeaponLaser : MonoBehaviour, IWeaponLaser
{
    [SerializeField] private float _range = 50f;
    [SerializeField] private TrailRenderer _trailRenderer;

    private IDamage _laserDamage;


    private void Awake()
    {
        TryGetComponent<IDamage>(out _laserDamage);
        
        _trailRenderer.enabled = false;
    }

    public void Shoot(Transform origin)
    {
        if (_laserDamage == null)
        {
            Debug.LogWarning($"No damage component found on '{gameObject.name}'.");
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(origin.position, origin.right, _range, ~(1 << gameObject.layer));
        //Debug.DrawRay(origin.position, origin.right * _range, Color.red, 5f);
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent<HealthSystem>(out var health))
            {
                health.HandleDamageCollision(hit.collider, _laserDamage);
            }

            SetTrailRenderer(origin.position, hit.point);
            return;
        }

        SetTrailRenderer(origin.position, origin.position + origin.right * _range);
    }

    private void SetTrailRenderer(Vector2 origin, Vector2 target)
    {
        _trailRenderer.transform.position = target;
        _trailRenderer.AddPosition(origin);
        _trailRenderer.AddPosition(target);
        _trailRenderer.enabled = true;

        Invoke(nameof(ClearTrailRenderer), 0.5f);
    }

    private void ClearTrailRenderer()
    {
        _trailRenderer.enabled = false;
        _trailRenderer.Clear();
    }
}