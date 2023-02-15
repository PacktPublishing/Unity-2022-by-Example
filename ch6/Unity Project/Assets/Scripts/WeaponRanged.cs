using UnityEngine;
using UnityEngine.Events;

public class WeaponRanged : MonoBehaviour, IWeapon
{
    [SerializeField]
    private ProjectileBase _bulletPrefab;
    public ProjectileBase BulletPrefab => _bulletPrefab;

    [SerializeField]
    private Transform _projectileSpawn;

    public void Shoot(ProjectileBase projectile, UnityAction<ProjectileBase> poolingReturnCallback)
    {
        projectile.transform.position = _projectileSpawn.position;
        projectile.Init(_projectileSpawn.right * transform.root.localScale.x, poolingReturnCallback);
    }
}
