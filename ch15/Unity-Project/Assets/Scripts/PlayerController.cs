using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private bool _shouldMoveForward;

    public Rigidbody2D Rb;
    public float MoveSpeed = 10f;
    public float SpriteRotationOffset = -90f;
    public float LookAtSpeed = 2f;

    // Added ch3 - Hitting Hazards
    [Header("Hit Hazard Speed")]
    public float SlowedSpeed = 2f;

    public float SlowedTime = 5f;   // Seconds
    private float _speedStartValue;

    // TODO: Cache the transform.

    #region Added ch15 - Themed graphics.

    [Header("Theme Graphics")]
    [SerializeField] private GameObject[] _graphics;

    private const string THEME_HOLIDAY = "Theme_Holiday";

    private void OnEnable()
        => RemoteConfigSettings.Instance.OnSettingsChanged += ConfigSettingsChanged;

    private void OnDisable()
        => RemoteConfigSettings.Instance.OnSettingsChanged -= ConfigSettingsChanged;

    private void ConfigSettingsChanged(RuntimeConfig config)
    {
        var isThemeEnabled = config.GetBool(THEME_HOLIDAY, false);
        if (!isThemeEnabled)
            return;

        ShowThemeGraphics(1);
    }
    
    private void ShowThemeGraphics(int value)
    {
        foreach (var g in _graphics)
        {
            g.SetActive(false);
        }
        _graphics[value].SetActive(true);
    }

    #endregion Added ch15 - Themed graphics.

    // Added ch3 - Hitting Hazards
    private void Start()
    {
        _speedStartValue = MoveSpeed;
    }

    // Update is called once per frame - process input
    private void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;                                                                  // Mouse added for bonus task.

        // Keyboard connected?
        if (keyboard == null || mouse == null)                                                      // Mouse connected for bonus task?
            return;                                                                                 // No keyboard connected so stop running code.

        if (keyboard.spaceKey.IsPressed() || mouse.leftButton.IsPressed())                          // Mouse condition added for bonus task.
        {
            // Move while holding spacebar key down.
            _shouldMoveForward = true;
        }
        else if (keyboard.spaceKey.wasReleasedThisFrame || mouse.leftButton.wasReleasedThisFrame)   // Mouse condition added for bonus task?
        {
            // The spacebar key was released - stop moving.
            _shouldMoveForward = false;
        }

        LookAtMousePointer();

        // HACK: Used for playable build outside the scope of the book.
#if !UNITY_EDITOR || TEST_DEPLOY
        if (keyboard.escapeKey.IsPressed())
        {
            Time.timeScale = 1.0f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
#endif
    }

    // FixedUpdate is called every physics fixed timestep.
    private void FixedUpdate()
    {
        if (_shouldMoveForward)
        {
            // Process physics movement.
            // Up is the direction the object sprite is currently facing.
            Rb.velocity = transform.up * MoveSpeed;
        }
        else
        {
            // Stop movement.
            Rb.velocity = Vector2.zero;
        }
    }

    private void LookAtMousePointer()
    {
        var mouse = Mouse.current;
        if (mouse == null)
            return;

        var mousePos = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());

        var direction = (Vector2)mousePos - Rb.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + SpriteRotationOffset;

        // Direct rotation.
        //Rb.rotation = angle;

        // Interpolated rotation – smoothed.
        // The forward (Z-axis) is want we want to rotate on.
        var q = Quaternion.AngleAxis(angle, Vector3.forward);
        Rb.transform.rotation = Quaternion.Slerp(Rb.transform.rotation, q, Time.deltaTime * LookAtSpeed);
    }

    // Added ch3 - Hitting Hazards
    public void SlowPlayerSpeed()
    {
        MoveSpeed = SlowedSpeed;
        Invoke(nameof(RestoreSpeed), SlowedTime);
    }

    // Added ch3 - Hitting Hazards
    private void RestoreSpeed()
    {
        MoveSpeed = _speedStartValue;
    }
}