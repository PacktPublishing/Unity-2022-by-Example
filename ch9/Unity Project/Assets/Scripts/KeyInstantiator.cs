using UnityEngine;
using System.Collections.Generic;

public class KeyInstantiator : MonoBehaviour
{
    [SerializeField] private KeyItem[] _keyPrefabs;
    [SerializeField] private Transform[] _spawnPoints;

    private List<Transform> _availablePoints;

    private void Start()
    {
        _availablePoints = new List<Transform>(_spawnPoints);

        foreach (var item in _keyPrefabs)
        {
            // Explain that Random.Range(int, int) is max EXCLUSIVE and works with array index range.
            var randomIndex = Random.Range(0, _availablePoints.Count);
            Instantiate(item,
                _availablePoints[randomIndex].position,
                Quaternion.identity);

            _availablePoints.RemoveAt(randomIndex);
        }
    }
}