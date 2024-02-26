using UnityEngine;
using UnityEngine.Events;

public class SensorTargetInFOV
{
    public event UnityAction<Transform> OnTargetDetected;

    private readonly MonoBehaviour _context;
    private Transform _target;

    private readonly float _fovAngle;
    private readonly float _fovRange;

    public SensorTargetInFOV(MonoBehaviour context, float fovAngle, float fovRange)
    {
        _context = context;
        _fovAngle = fovAngle;
        _fovRange = fovRange;
    }

    public bool IsTargetInsideFOV()
    {
        var directionToTarget = _target.position - _context.transform.position;
        var angle = Vector3.Angle(directionToTarget, _context.transform.forward);

        Debug.DrawRay(_context.transform.position, directionToTarget.normalized * _fovRange, Color.red);

        if (angle < _fovAngle * 0.5f)
        {
            var sqrDistanceToTarget = directionToTarget.sqrMagnitude;
            var sqrFOVRange = _fovRange * _fovRange;

            if (sqrDistanceToTarget <= sqrFOVRange)
            {
                var layerMask = 1 << LayerMask.NameToLayer(Tags.Player);
                if (Physics.Raycast(
                    _context.transform.position,
                    directionToTarget.normalized,
                    out RaycastHit hit,
                    _fovRange,
                    layerMask))
                {
                    //Debug.Log($"Angle: {angle}, Distance: {sqrDistanceToTarget}, Hit: {hit.transform?.name}");

                    if (hit.transform == _target || hit.transform.IsChildOf(_target))
                    {
                        OnTargetDetected?.Invoke(_target);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void Tick() => IsTargetInsideFOV();

    public void SetTarget(Transform target) => _target = target;
}