using UnityEngine;

public class LaserAttack : MonoBehaviour, IBehaviorAttack
{
    [SerializeField] private float _cooldown = 2.5f;
    private float _nextShootTime;

    private IWeaponLaser _weapon;
    private Transform _laserOrigin;


    private void Awake()
        => TryGetComponent<IWeaponLaser>(out _weapon);

    public void Init(Transform origin)=> _laserOrigin = origin;

    public void TickPhysics()
    {
        if (Time.time >= _nextShootTime)
            Shoot();
    }

    private void Shoot()
    {
        _nextShootTime = Time.time + _cooldown;
        _weapon?.Shoot(_laserOrigin);
    }
}