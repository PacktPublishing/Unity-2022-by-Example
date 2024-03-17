using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyConfigData", menuName = "ScriptableObjects/EnemyConfigData")]
public class EnemyConfigData : ScriptableObject
{
    public float Acceleration, SpeedMax;
    public float AttackRange, FireRange, FireCooldown;
    public bool CanJump;
    public float JumpForce;

    [Header("Behavior Properties")]
    public float TimeIdle = 5f;
    public float TimePatrol = 15f;
}
