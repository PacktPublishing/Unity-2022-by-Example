using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyConfigData _config;

    public enum State
    { Idle, Patrol, Attack, Dead }

    public State CurrentState => _currentState;

    [SerializeField]    // TEMP: For debugging purposes only.
    private State _currentState;

    private float _timeStateStart;

    [SerializeField] private Transform _shootFromPosition;

    private static GameObject _player;

    // Implemented behaviors.
    private IBehaviorMovement _behaviorMove;

    private IBehaviorAttack _behaviorAttack;

    // Implemented sensors.
    private SensorHearing _sensorHearing;

    private SensorTargetInFOV _sensorTargetInFOV;

    // UNDONE: Temporary variable until a HealthSystem component is attached?
    [SerializeField] private int _health = 10;

    private void Awake()
    {
        if (_player == null)
            _player = GameObject.FindWithTag(Tags.Player);

        // Get behaviors and initialize.
        if (TryGetComponent<IBehaviorMovement>(out _behaviorMove))
        {
            _behaviorMove.Init(_config.SpeedMax);
        }

        if (TryGetComponent<IBehaviorAttack>(out _behaviorAttack))
        {
            _behaviorAttack.Init(_shootFromPosition);
        }
    }

    private void Start() => ChangeState(State.Idle);

    private void Update()
    {
        // FSM - Evaluate state.
        switch (_currentState)
        {
            case State.Idle:
                // UNDONE: Do stuff --> change state?

                // Is the player in range? If so, attack.
                if (IsPlayerInRange(_config.AttackRange))
                    ChangeState(State.Attack);
                else if (Time.time - _timeStateStart >= _config.TimeIdle)
                    ChangeState(GetNextState(_currentState));
                break;

            case State.Patrol:
                // UNDONE: Do stuff --> change state?
                // Move between patrol waypoints.

                // Is the player in range? If so, attack.
                if (IsPlayerInRange(_config.AttackRange))
                    ChangeState(State.Attack);
                else if (Time.time - _timeStateStart >= _config.TimePatrol)
                    ChangeState(GetNextState(_currentState));
                break;

            case State.Attack:
                // UNDONE: Do stuff --> change state?
                // Shoot with cooldown.

                // If the player is out of range, stop shooting and return to patrolling.
                if (!IsPlayerInRange(_config.AttackRange))
                    ChangeState(GetNextState(_currentState));
                break;

            case State.Dead:
                Destroy(gameObject);
                break;
        }

        // Any state.
        if (_health <= 0)
        {
            // UNDONE: Add an event listener to the HealthSystem for OnDead. Or, use SendMessage() in HealthSystem for calling a "Died" method.
            ChangeState(State.Dead);
        }
    }

    private void FixedUpdate()
    {
        if (_currentState == State.Patrol)
        {
            _behaviorMove?.TickPhysics();
        }
        else if (_currentState == State.Attack)
        {
            _behaviorAttack?.TickPhysics();
        }
        //else
        //    _rb.velocity = Vector2.zero;
    }

    private void OnDestroy()
    {
        if (_currentState == State.Dead)
        {
            // UNDONE: Do on dead stuff.
        }
    }

    public void ChangeState(State state)
    {
        _currentState = state;
        _timeStateStart = Time.time;

        // Perform "on state enter" required actions.
        SetAnimationForState(state);
    }

    // ???
    private State GetNextState(State currentState) => currentState switch
    {
        State.Idle => State.Patrol,
        State.Patrol => State.Idle,
        State.Attack => State.Patrol,
        _ => State.Dead
        // Q: What is the proper explanation for the discard variable _ (underscore) in a switch expression?
        // A: The discard pattern can be used in pattern matching with the switch expression where
        //    every expression, including null, always matches the discard pattern.
    };

    private void SetAnimationForState(State state)
    {
        switch (state)
        {
            case State.Idle:
                //_animator.SetTrigger("Idle");
                break;

            case State.Patrol:
                //_animator.SetTrigger("Patrol");
                break;

            case State.Attack:
                // Set the enemy to face the player.
                //_animator.SetTrigger("Attack");
                break;

            case State.Dead:
                //_animator.SetTrigger("Dead");
                break;
        }
    }

    private bool IsPlayerInRange(float rangeAttack)
    {
        var distance = Vector3.Distance(transform.position, _player.transform.position);
        return distance <= rangeAttack;
    }
}