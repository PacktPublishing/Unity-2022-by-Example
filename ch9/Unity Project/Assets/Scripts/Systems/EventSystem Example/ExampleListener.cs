using UnityEngine;

public class ExampleListener : MonoBehaviour
{
    private void Start()
        => EventSystem.Instance.AddListener<int>(EventConstants.MyFirstEvent, ExampleEventHandler);

    private void OnDestroy()
        => EventSystem.Instance.RemoveListener<int>(EventConstants.MyFirstEvent, ExampleEventHandler);

    private void ExampleEventHandler(int arg)
        => Debug.Log($"Example event handled with arg: {arg}");
}