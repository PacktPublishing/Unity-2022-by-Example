using UnityEngine;

public class MoveToTarget : MonoBehaviour, IBehaviorMovement
{
    private Transform _transformTarget;
    private float _moveSpeed;

    public void Init(float speed)
    {
        _moveSpeed = speed;
    }

    public void SetTarget(Transform target)
    {
        _transformTarget = target;
    }

    public void TickPhysics() => UpdateMovement();

    private void UpdateMovement()
    {
        if (_transformTarget != null)
        {
            var step = _moveSpeed * Time.deltaTime;
            gameObject.transform.position = Vector3.MoveTowards(
                gameObject.transform.position, _transformTarget.position, step);
        }
    }
}