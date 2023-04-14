using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private float _spawnInterval = 5f;

    //[SerializeField] private Vector2 SpawnIntervalMinMax = new(5f, 10f);
    [SerializeField] private int _maxSpawned = 3;

    [Header("Patrol Waypoints")]
    [SerializeField] private Transform _waypointPatrolLeft;
    [SerializeField] private Transform _waypointPatrolRight;

    private int _objectCount = 0;


    private void Start()
        => InvokeRepeating(
            nameof(SpawnEnemy), 0f, _spawnInterval);
    //Invoke(nameof(SpawnEnemy),//        Random.Range(SpawnIntervalMinMax.x, SpawnIntervalMinMax.y));


    private void SpawnEnemy()
    {
        if (_objectCount < _maxSpawned)
        {
            var enemy = Instantiate(_enemyPrefab,
                transform.position, Quaternion.identity);
            enemy.Init(DestroyedCallback);

            _objectCount++;

            if (enemy.TryGetComponent<IBehaviorPatrolWaypoints>(out var patrol))
            {
                patrol.SetWaypoints(_waypointPatrolLeft, _waypointPatrolRight);
            }
        }

        //float nextSpawnTime = Random.Range(SpawnIntervalMinMax.x, SpawnIntervalMinMax.y);
        //Invoke(nameof(SpawnEnemy), nextSpawnTime);
    }

    public void DestroyedCallback() => _objectCount--;
}