using UnityEngine;
using UnityEngine.Events;

public abstract class ProjectileBase : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _velocity = 30f;
    [SerializeField] private float _lifetime = 2f;

    private event UnityAction<ProjectileBase> _onCollisionAction;

    // To cache the TrailRenderer component for tweaking out more frame optimization
    // (without having to do GetComponent() every time the bullet is spawned from the pooler,
    // uncomment the below line.
    //private TrailRenderer _trailRenderer;

    // To use the cached TrailRenderer component instead, uncomment the below lines.
    //private void Awake() => _trailRenderer = GetComponent<TrailRenderer>();

    public virtual void Init(Vector3 direction, UnityAction<ProjectileBase> collisionCallback)
    {
        // If there is a TrailRenderer component on this GameObject reset it.
        if (TryGetComponent<TrailRenderer>(out var tr))
            tr.Clear();

        // To use the cached TrailRenderer component instead, uncomment the below lines.
        //if (_trailRenderer != null)
        //    _trailRenderer.Clear();

        _onCollisionAction = collisionCallback;

        _rb.velocity = direction * _velocity;
        Invoke(nameof(LifetimeExpired), _lifetime);
    }

    protected abstract void OnTriggerEnter(Collider collision);
    protected abstract void LifetimeExpired();

    protected virtual void Collided()
    {
        CancelInvoke();
        _onCollisionAction?.Invoke(this);
    }
}
