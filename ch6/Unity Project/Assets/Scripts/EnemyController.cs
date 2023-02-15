using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyConfigData _config;

    public enum State { Idle, Patrol, Attack, Dead }
    public State CurrentState => _currentState;
    [SerializeField]    // TEMP: For debugging purposes only.
    private State _currentState;
    private float _timeStateStart;

    private static GameObject _player;
    private Rigidbody2D _rb;

    // Set to the default direction the Enemy graphics are facing in the Scene.
    private Vector2 _movementDirection = Vector2.right;
    
    // Implemented behaviors.
    private IBehaviorPatrolWaypoints _behaviorPatrol;

    [SerializeField]
    private int _health = 10;


    private void Awake()
    {
        if (_player == null)
            _player = GameObject.FindWithTag(Tags.Player);

        _rb = GetComponent<Rigidbody2D>();

        // Get behaviors and initialize.
        if (TryGetComponent<IBehaviorPatrolWaypoints>(out _behaviorPatrol))
        {
            _behaviorPatrol.Init(_rb, _movementDirection,
                _config.Acceleration, _config.SpeedMax);
        }
    }

    private void Start() => ChangeState(State.Idle);

    void Update()
    {
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
            print("EnemyController 'Attack' behavior not implemented!");
            //_behaviorAttack?.TickPhysics();
        }
        else
            _rb.velocity = Vector2.zero;
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

        // Perform "on state enter" requiried actions.
        SetAnimationForState(state);
    }

    // ???
    private State GetNextState(State currentState) => currentState switch
    {
        State.Idle => State.Patrol,
        State.Patrol => State.Idle,
        State.Attack => State.Patrol,
        _ => State.Dead                     
        // Q: What is the proper explaination for the discard variable _ (underscore) in a switch expression?
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
        var distance = Vector2.Distance(transform.position, _player.transform.position);
        return distance <= rangeAttack;
    }
}
