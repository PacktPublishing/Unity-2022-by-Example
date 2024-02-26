using UnityEngine;

public class Bullet : ProjectileBase
{
    [SerializeField] private LayerMask _collideWith;


    protected override void OnTriggerEnter(Collider collision)
    {
        // Use the | operator instead of & operator for Default layer (0) to return true.
        if ((_collideWith & (1 << collision.gameObject.layer)) != 0)
            base.Collided();
    }

    protected override void LifetimeExpired() => base.Collided();
}
