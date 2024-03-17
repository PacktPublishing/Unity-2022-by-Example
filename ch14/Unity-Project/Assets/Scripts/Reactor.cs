using UnityEngine;

public class Reactor : MonoBehaviour
{
    [SerializeField] private Material _matOffline;
    [SerializeField] private Material _matEnergized;

    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<MeshRenderer>();
        _renderer.material = _matOffline;
    }

    private void OnEnable()
        => EventSystem.Instance.AddListener<bool>(EventConstants.OnConsoleEnergized, Energize);

    private void OnDestroy()
        => EventSystem.Instance.RemoveListener<bool>(EventConstants.OnConsoleEnergized, Energize);

    public void Energize(bool energize)
        => _renderer.material = _matEnergized;
}