using UnityEngine;

public class Bullet : ProjectileBase
{
    [SerializeField] private LayerMask CollideWith;


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // use | operator instead of & operator for Default layer (0) to return true
        if ((CollideWith & (1 << collision.gameObject.layer)) != 0)
            base.Collided();
    }
    protected override void LifetimeExpired() => base.Collided();
}
