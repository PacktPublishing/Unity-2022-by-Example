using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    private ARPlaneManager _planeManager;

    [Header("Triggered Events")]
    public UnityEvent<bool> OnTogglePassthrough;

    private bool _isPassthroughVisible = false;
    private bool _arePlanesVisible = true;

    public void TogglePassthrough()
    {
        _isPassthroughVisible = !_isPassthroughVisible;
        SetPassthroughVisible(_isPassthroughVisible);
    }

    public void SetPassthroughVisible(bool visible)
    {
        _isPassthroughVisible = visible;
        OnTogglePassthrough.Invoke(_isPassthroughVisible);
    }

    public void TogglePlaneVisibility()
    {
        _arePlanesVisible = !_arePlanesVisible;
        SetPlaneVisible(_arePlanesVisible);
    }

    public void SetPlaneVisible(bool visible)
    {
        _arePlanesVisible = visible;

        foreach (var plane in _planeManager.trackables)
        {
            if (plane.gameObject.TryGetComponent<FadePlaneMaterial>(out var planeFader))
            {
                planeFader.FadePlane(_arePlanesVisible);
            }
        }
    }
}