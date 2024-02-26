using UnityEngine;
using UnityEngine.Events;

public class SensorTemplate
{
    public event UnityAction OnSensorDetected;

    private readonly MonoBehaviour _context;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context"></param>
    public SensorTemplate(MonoBehaviour context)
    {
        // Initialize.
        _context = context;
    }

    /// <summary>
    /// Called every frame from the implementing class.
    /// </summary>
    public void Tick()
    {
        // Invoke only if detection occurred.
        OnSensorDetected?.Invoke();
    }
}