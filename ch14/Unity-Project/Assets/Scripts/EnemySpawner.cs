using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Transform _targetMoveTo;

    [SerializeField] private float _tickInterval = 2f;
    [SerializeField] private Vector2 _randomInitializeTime = new Vector2(2f, 6f);

    private Enemy _currentInstance;

    private void Start() => StartCoroutine(SpawnEnemyWithDelay());

    private IEnumerator SpawnEnemyWithDelay()
    {
        while (true)
        {
            while (_currentInstance != null)
            {
                yield return new WaitForSeconds(_tickInterval);
            }

            _currentInstance = Instantiate(_enemyPrefab, transform.position, transform.rotation);
            _currentInstance.Init(OnEnemyDestroyed);

            yield return new WaitForSeconds(
                Random.Range(_randomInitializeTime.x, _randomInitializeTime.y));

            if (_currentInstance != null)
            {
                _currentInstance.GetComponent<IBehaviorMovement>().SetTarget(_targetMoveTo); 
            }
        }
    }

    private void OnEnemyDestroyed()
    {
        Debug.Log($"[{nameof(EnemySpawner)}] Spawner <Enemy> destroyed callback triggered.");
        _currentInstance = null;
    }
}