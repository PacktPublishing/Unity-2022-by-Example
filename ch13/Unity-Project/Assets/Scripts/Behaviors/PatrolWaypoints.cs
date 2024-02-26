using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolWaypoints : MonoBehaviour, IBehaviorPatrolWaypoints
{
    // NOTE: Cycle through the waypoints in order!
    [Header("Patrol Waypoints")]
    [Tooltip("Arrange the waypoint to cycle through them in order.")]
    [SerializeField] private List<Transform> _waypoints;

    private int _waypointCurrentIndex = 0;
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        // We cannot easily add the NavMesh agent component while requiring assigning the Agent Type (refer to the AI Navigation samples concerning runtime assignment of the Agent Type).
        Debug.Assert(_navMeshAgent != null,
            $"[{nameof(PatrolWaypoints)} NavMesh agent is null!]", gameObject);
    }

    public void Init(float acceleration, float speedMax)
    {
        _navMeshAgent.acceleration = acceleration;
        _navMeshAgent.speed = speedMax;

        if (_waypoints.Count > 0)
        {
            _navMeshAgent.SetDestination(_waypoints[0].position);
        }

        _waypointCurrentIndex = 0;
    }

    public void TickPhysics() => UpdateDirection();

    private void UpdateDirection()
    {
        // Rotation will automatically be handled by the NavMeshAgent but if we'll implement our own rotation logic then ensure the agent's update rotation is disabled.
        //_navMeshAgent.updateRotation = false;

        // A good option to add to the NavMesh agent, is for it to wait at the waypoint before moving onto the next. A field in the Inspector for the wait time would be a good addition also.
       
        if (!_navMeshAgent.pathPending
            && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            MoveToNextWaypoint();
        }
    }

    private void MoveToNextWaypoint()
    {
        // Alternative ways to write the evaluation for ensuring we have available waypoints in the array.
        //if (_waypoints == null || _waypoints.Count == 0) return;
        //if ((_waypoints?.Count ?? 0) == 0) return;

        if (!(_waypoints?.Count > 0))
            return;

        _waypointCurrentIndex =
            (_waypointCurrentIndex + 1) % _waypoints.Count;
        _navMeshAgent.SetDestination(_waypoints[_waypointCurrentIndex].position);
    }
}