using UnityEngine;
using UnityEngine.Events;

public class WeaponRanged : MonoBehaviour, IWeapon
{
    public ProjectileBase BulletPrefab => _bulletPrefab;
    
    [SerializeField] private ProjectileBase _bulletPrefab;
    [SerializeField] private Transform _projectileSpawn;

    public void Shoot(ProjectileBase projectile, UnityAction<ProjectileBase> poolingReturnCallback)
    {
        projectile.transform.position = _projectileSpawn.position;
        projectile.Init(_projectileSpawn.forward, poolingReturnCallback);
    }
}
