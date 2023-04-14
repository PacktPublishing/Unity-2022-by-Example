using UnityEngine;

public class ExampleTrigger : MonoBehaviour
{
    [ContextMenu("Trigger Event")]
    public void TriggerEvent()
        => EventSystem.Instance.TriggerEvent(EventConstants.MyFirstEvent, 100);
}
