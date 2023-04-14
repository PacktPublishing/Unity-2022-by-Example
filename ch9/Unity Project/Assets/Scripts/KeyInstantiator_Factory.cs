using UnityEngine;
using System.Collections.Generic;

public class KeyInstantiator_Factory : MonoBehaviour
{
    [SerializeField] private KeyItemData[] _keyData;
    [SerializeField] private Transform[] _spawnPoints;

    private List<Transform> _availablePoints;

    private void Start()
    {
        _availablePoints = new List<Transform>(_spawnPoints);

        foreach (var item in _keyData)
        {
            // Explain that Random.Range(int, int) is max EXCLUSIVE and works with array index range.
            int randomIndex = Random.Range(0, _availablePoints.Count);
            Vector3 position = _availablePoints[randomIndex].position;
            Quaternion rotation = Quaternion.identity;

            using var factory = new KeyItemFactory();
            factory.CreateKeyItem(item, position, rotation);

            _availablePoints.RemoveAt(randomIndex);
        }
    }
}