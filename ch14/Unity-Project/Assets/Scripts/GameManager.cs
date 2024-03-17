using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SceneController))]
public class GameManager : MonoBehaviour
{
    private enum State
    { Menu, Playing, GameOver }

    private State _currentState;
    private bool _isConditionMetWin;
    private bool _isConditionMetLose;

    [SerializeField]
    private ARPlaneManager _planeManager;

    [SerializeField]
    private bool _enablePlaneManagerOnStart = false;

    [Header("Boss Room Elements to Spawn")]
    [SerializeField] private GameObject _prefabReactor;

    [SerializeField] private GameObject _prefabConsole;
    [SerializeField] private GameObject _prefabCorridorDoorway;
    [SerializeField] private GameObject _prefabGun;
    [SerializeField] private GameObject[] _prefabModules;

    //[Header("Boss Room Configuration")]

    //[Header("Debug")]
    //[SerializeField] private bool _showDebugWidgets = false;

    //[SerializeField] private GameObject _prefabDebugCube;

    private readonly List<ARPlane> _detectedWalls = new();

    private bool _hasSpawnedPrefab_Reactor = false;
    private bool _hasSpawnedPrefab_Console = false;
    private bool _hasSpawnedPrefab_Corridor = false;

    private SceneController _sceneController;

    private void Awake()
        => _sceneController = GetComponent<SceneController>();

    private IEnumerator Start()
    {
        if (_enablePlaneManagerOnStart)
        {
            yield return new WaitForSeconds(2f);
            EnablePlaneManager();
        }
    }

    private void OnEnable()
    {
        EventSystem.Instance.AddListener<bool>(
            EventConstants.OnPlayerDied, SetLoseCondition);

        EventSystem.Instance.AddListener<bool>(
            EventConstants.OnConsoleEnergized, SetWinCondition);
    }

    private void OnDisable()
    {
        EventSystem.Instance.RemoveListener<bool>(
            EventConstants.OnPlayerDied, SetLoseCondition);

        EventSystem.Instance.RemoveListener<bool>(
            EventConstants.OnConsoleEnergized, SetWinCondition);
    }

    private void SetLoseCondition(bool value) => _isConditionMetLose = value;

    private void SetWinCondition(bool value) => _isConditionMetWin = value;

    private void OnDestroy()
        => _planeManager.planesChanged -= OnPlanesChanged;

    private void Update()
    {
        switch (_currentState)
        {
            case State.Playing:
                if (_isConditionMetWin || _isConditionMetLose)
                {
                    ChangeState(State.GameOver);
                }
                break;
        }
    }

    public void StartGame()
    {
        EnablePlaneManager();
        StartCoroutine(DelayStartGame());

        IEnumerator DelayStartGame()
        {
            yield return new WaitForSeconds(1.5f);

            _sceneController.SetPlaneVisible(false);
            _sceneController.SetPassthroughVisible(true);
        }
    }

    private void EndGame()
    {
        if (_isConditionMetWin)
        {
            // UNDONE: Invoke event.
            return;
        }

        if (_isConditionMetLose)
        {
            // UNDONE: Invoke event.
            return;
        }

        Time.timeScale = 0;
    }

    public void EnablePlaneManager()
    {
        if (_planeManager.enabled)
            return;

        //Debug.Log($"[{nameof(SceneController)}] Enabling ARPlaneManager");
        _planeManager.enabled = true;
        _planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        //Debug.Log($"[{nameof(SceneManager)}] OnPlanesChanged() called.");

        _detectedWalls.Clear();

        foreach (var plane in args.added)
        {
            //if (_showDebugWidgets)
            //{
            //    AddDebugLines(plane);
            //}

            switch (plane.classification)
            {
                case PlaneClassification.Wall:

                    //if (!_hasSpawnedPrefab_Corridor)
                    //{
                    //    SpawnPrefabAtWallBase(plane, _prefabCorridorDoorway);
                    //    _hasSpawnedPrefab_Corridor = true;
                    //}
                    //break;

                    _detectedWalls.Add(plane);
                    break;

                case PlaneClassification.Floor:

                    if (!_hasSpawnedPrefab_Console)
                    {
                        SpawnPrefab(plane, _prefabConsole, new Vector3(-0.8f, 0f, 0f));
                        _hasSpawnedPrefab_Console = true;

                        // Also spawn the modules.
                        SpawnPrefab(_prefabModules,
                            new Vector3(0f, 1.5f, 0.8f),
                            Vector3.up, 0.05f);

                        // Also spawn the gun.
                        SpawnPrefab(_prefabGun, new Vector3(0.5f, 1.2f, 0.15f));
                    }
                    break;

                case PlaneClassification.Table:

                    if (!_hasSpawnedPrefab_Reactor)
                    {
                        SpawnPrefab(plane, _prefabReactor);
                        _hasSpawnedPrefab_Reactor = true;
                    }
                    break;

                default:
                    break;
            }
        }

        if (!_hasSpawnedPrefab_Corridor)
        {
            DetectAndSpawnForLargestWalls();
            _hasSpawnedPrefab_Corridor = true;
        }
    }

    #region State methods

    private void ChangeState(State state) => _currentState = state;

    private State GetNextState(State currentState) => currentState switch
    {
        State.Menu => State.Playing,
        State.Playing => State.GameOver,
        _ => State.Playing
    };

    #endregion State methods

    #region Spawn Methods

    private void SpawnPrefab(ARPlane plane, GameObject prefab)
    {
        Debug.Log($"[{nameof(GameManager)}] Spawning '{plane.gameObject.name}'");
        Instantiate(prefab,
            plane.transform.position,
            plane.transform.rotation);
    }

    private void SpawnPrefab(ARPlane plane, GameObject prefab, Vector3 playerOffset)
    {
        Debug.Log($"[{nameof(GameManager)}] Spawning '{plane.gameObject.name}'");

        var playerTransform = Camera.main.transform;

        // Ensure the prefab is instantiated sitting on the plane surface with the remainder of the values coming from the camera offset.
        playerTransform.position = new Vector3(
            playerTransform.position.x,
            plane.transform.position.y,
            playerTransform.position.z);

        var worldOffset = playerTransform.TransformDirection(playerOffset);
        var spawnPosition = playerTransform.position + worldOffset;

        var directionToPlayer = (playerTransform.position - spawnPosition).normalized;
        var spawnRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
        Instantiate(prefab, spawnPosition, spawnRotation);
    }

    private void SpawnPrefab(GameObject prefab, Vector3 playerOffset)
        => SpawnPrefab(new GameObject[] { prefab },
            playerOffset, Vector3.zero, 0f);

    private void SpawnPrefab(GameObject[] prefabs, Vector3 playerOffset,
        Vector3 forceDirection, float force)
    {
        var playerTransform = Camera.main.transform;

        var spawnPosition = new Vector3(
            playerTransform.position.x + (playerTransform.right * playerOffset.x).x,
            playerTransform.position.y + (playerTransform.up * playerOffset.y).y,
            playerTransform.position.z + (playerTransform.forward * playerOffset.z).z
        );

        foreach (var item in prefabs)
        {
            Debug.Log($"[{nameof(GameManager)}] Spawning '{item.name}'");

            var module = Instantiate(item, spawnPosition, Quaternion.identity);

            if (forceDirection != Vector3.zero || force != 0)
            {
                if (module.TryGetComponent<Rigidbody>(out var rb))
                {
                    ApplyForce(rb);
                }
            }
        }

        void ApplyForce(Rigidbody rb)
        {
            rb.AddForce(forceDirection * force, ForceMode.Impulse);

            var torqueMultiplier = 3f;
            var randomRotation = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)).normalized * (force * torqueMultiplier);
            rb.AddTorque(randomRotation, ForceMode.Impulse);
        }
    }

    private void SpawnPrefabAtWallBase(ARPlane plane, GameObject prefab)
    {
        var planeCenter = plane.transform.position;
        var heightOffset = plane.extents.y;
        var basePosition = new Vector3(planeCenter.x, planeCenter.y - heightOffset, planeCenter.z);
        var prefabRotation = Quaternion.LookRotation(-plane.normal, Vector3.up);

        Instantiate(prefab, basePosition, prefabRotation);
    }

    private void DetectAndSpawnForLargestWalls()
    {
        _detectedWalls.Sort((a, b)
            => (b.extents.x * b.extents.y).CompareTo(a.extents.x * a.extents.y));

        if (_detectedWalls.Count >= 2 && !_hasSpawnedPrefab_Corridor)
        {
            //if (_showDebugWidgets)
            //{
            //    AddDebugCubes(_detectedWalls[0]);
            //}
            SpawnPrefabAtWallBase(_detectedWalls[0], _prefabCorridorDoorway);

            //if (_showDebugWidgets)
            //{
            //    AddDebugCubes(_detectedWalls[1]);
            //}
            SpawnPrefabAtWallBase(_detectedWalls[1], _prefabCorridorDoorway);
        }
    }

    #endregion Spawn Methods

    //#region Debug

    //private void AddDebugCubes(ARPlane plane)
    //{
    //    Quaternion planeRotation = plane.transform.rotation;

    //    // Instantiate a cube at the center of the plane for visualization, aligned with the plane's rotation.
    //    Instantiate(_prefabDebugCube, plane.transform.position, planeRotation, plane.transform);

    //    // Using extents to calculate corner positions in local space of the plane.
    //    var extents = plane.extents; // Extents are half the size of each dimension.

    //    // Calculate corners using extents. Note: Assuming Y is up, adjust if necessary for your coordinate system.
    //    var bottomLeft = new Vector3(-extents.x, 0, -extents.y);
    //    var bottomRight = new Vector3(extents.x, 0, -extents.y);
    //    var topLeft = new Vector3(-extents.x, 0, extents.y);
    //    var topRight = new Vector3(extents.x, 0, extents.y);

    //    // Instantiate debug cubes at each corner, transforming local positions to world positions.
    //    Instantiate(_prefabDebugCube, plane.transform.TransformPoint(bottomLeft), planeRotation, plane.transform);
    //    Instantiate(_prefabDebugCube, plane.transform.TransformPoint(bottomRight), planeRotation, plane.transform);
    //    Instantiate(_prefabDebugCube, plane.transform.TransformPoint(topLeft), planeRotation, plane.transform);
    //    Instantiate(_prefabDebugCube, plane.transform.TransformPoint(topRight), planeRotation, plane.transform);
    //}

    //public void AddDebugLines(ARPlane plane)
    //{
    //    // Create a new GameObject for the forward direction line
    //    var forwardLineObj = new GameObject("ForwardLine");
    //    var forwardLineRenderer = forwardLineObj.AddComponent<LineRenderer>();

    //    // Set up the LineRenderer
    //    SetupLineRenderer(forwardLineRenderer, Color.blue);

    //    // Position the line from the center of the plane forward
    //    var forwardStart = plane.transform.position;
    //    var forwardEnd = forwardStart + (plane.transform.forward * 1f);
    //    forwardLineRenderer.SetPosition(0, forwardStart);
    //    forwardLineRenderer.SetPosition(1, forwardEnd);

    //    // Create a new GameObject for the normal line
    //    var normalLineObj = new GameObject("NormalLine");
    //    var normalLineRenderer = normalLineObj.AddComponent<LineRenderer>();

    //    // Set up the LineRenderer
    //    SetupLineRenderer(normalLineRenderer, Color.white);

    //    // Position the line from the center of the plane along its normal
    //    var normalStart = plane.transform.position;
    //    var normalEnd = normalStart - (plane.normal * 0.5f);
    //    normalLineRenderer.SetPosition(0, normalStart);
    //    normalLineRenderer.SetPosition(1, normalEnd);
    //}

    //private void SetupLineRenderer(LineRenderer lineRenderer, Color color)
    //{
    //    lineRenderer.startWidth = 0.05f;
    //    lineRenderer.endWidth = 0.05f;
    //    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    //    lineRenderer.startColor = color;
    //    lineRenderer.endColor = color;
    //    lineRenderer.positionCount = 2;
    //}

    //#endregion Debug
}