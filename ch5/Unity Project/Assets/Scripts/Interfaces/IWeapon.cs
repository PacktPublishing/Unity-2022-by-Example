using UnityEngine.Events;

internal interface IWeapon
{
    ProjectileBase BulletPrefab { get; }
    void Shoot(ProjectileBase projectile, UnityAction<ProjectileBase> poolingReleaseCallback);
}