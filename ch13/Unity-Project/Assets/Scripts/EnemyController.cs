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
    //private Rigidbody2D _rb;

    // Set to the default direction the Enemy graphics are facing in the Scene.
    //private Vector2 _movementDirection = Vector2.right;

    // Implemented behaviors.
    private IBehaviorPatrolWaypoints _behaviorPatrol;

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

        //_rb = GetComponent<Rigidbody2D>();

        // Get behaviors and initialize.
        if (TryGetComponent<IBehaviorPatrolWaypoints>(out _behaviorPatrol))
        {
            //_behaviorPatrol.Init(_rb, _movementDirection,
            //    _config.Acceleration, _config.SpeedMax);
            _behaviorPatrol.Init(_config.Acceleration, _config.SpeedMax);
        }

        if (TryGetComponent<IBehaviorAttack>(out _behaviorAttack))
        {
            _behaviorAttack.Init(_shootFromPosition);
        }
    }

    private void Start()
    {
        // Implement sensors.
        _sensorTargetInFOV = new SensorTargetInFOV(this, 110f, 20f);
        _sensorTargetInFOV.OnTargetDetected += HandleSensor_TargetDetected;

        _sensorHearing = new SensorHearing(this, 30f, 2f);
        _sensorHearing.OnAudioDetected += HandleSensor_AudioDetected;

        ChangeState(State.Idle);
    }

    private void Update()
    {
        // Update player target for FOV sensor.
        _sensorTargetInFOV.SetTarget(_player.transform);

        // Tick sensors.
        _sensorTargetInFOV.Tick();
        _sensorHearing.Tick();

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
            _behaviorPatrol?.TickPhysics();
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
        // Cleanup the sensor event handlers.
        _sensorTargetInFOV.OnTargetDetected -= HandleSensor_TargetDetected;
        _sensorHearing.OnAudioDetected -= HandleSensor_AudioDetected;

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

    #region Handle sensors.

    private void HandleSensor_TargetDetected(Transform transform)
    {
        print("Enemy NPC detected Player in FOV!");
    }

    private void HandleSensor_AudioDetected(AudioSource source)
    {
        print("Enemy NPC detected audio!");
    }

    #endregion Handle sensors.
}
