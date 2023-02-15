using UnityEngine;
using UnityEngine.Pool;

public class PlayerShootingPooled : MonoBehaviour
{
    [SerializeField] private Transform _weaponAttachment;

    [Header("Weapon")]
    [SerializeField] private WeaponRanged _weapon1;

    [Header("Projectile Pooling")]
    public int PoolDefaultCapacity = 10;
    public int PoolMaxSize = 25;

    private ObjectPool<ProjectileBase> _poolProjectiles;


    private void Start()
    {
        _poolProjectiles = new ObjectPool<ProjectileBase>(
            CreatePooledItem, OnGetFromPool, OnReturnToPool, OnDestroyPoolItem,
            collectionCheck: false,
            PoolDefaultCapacity, PoolMaxSize);

        ProjectileBase CreatePooledItem() => Instantiate(_weapon1.BulletPrefab);
        void OnGetFromPool(ProjectileBase projectile) => projectile.gameObject.SetActive(true);
        void OnReturnToPool (ProjectileBase projectile) => projectile.gameObject.SetActive(false);
        void OnDestroyPoolItem(ProjectileBase projectile) => Destroy(projectile.gameObject);
    }

    private void OnFire() => _weapon1.Shoot(_poolProjectiles.Get(), ReturnProjectile);

    private void ReturnProjectile(ProjectileBase projectile) => _poolProjectiles.Release(projectile);
}
