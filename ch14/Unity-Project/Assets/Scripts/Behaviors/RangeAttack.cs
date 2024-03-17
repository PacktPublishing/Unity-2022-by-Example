using UnityEngine;

public class RangeAttack : MonoBehaviour, IBehaviorAttack
{
    [SerializeField] private float _cooldown = 2.5f;
    private float _nextShootTime;

    public void Init(Transform origin)
    {
        // For PlayerShootingPooled the shooting transform position is assigned in the IWeapon implementing class.
    }

    public void TickPhysics()
    {
        if (Time.time >= _nextShootTime)
            Shoot();
    }

    private void Shoot()
    {
        _nextShootTime = Time.time + _cooldown;
        
        // Simulate the PlayerInput system by sending message of OnFire event triggered - PlayerShootingPooled listens.
        SendMessage("OnFire");
    }
}