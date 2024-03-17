using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ConsoleSlot : MonoBehaviour
{
    [SerializeField] private char _slotID;

    private ConsoleController _controller;
    private XRSocketInteractor _socketInteractor;

    private char _moduleID;

    private void Awake()
    {
        _controller = GetComponentInParent<ConsoleController>();
        _socketInteractor = GetComponent<XRSocketInteractor>();

        _socketInteractor.selectEntered.AddListener(HandleModuleInserted);
        _socketInteractor.selectExited.AddListener(HandleModuleRemoved);
    }

    private void OnDestroy()
    {
        _socketInteractor.selectEntered.RemoveListener(HandleModuleInserted);
        _socketInteractor.selectExited.RemoveListener(HandleModuleRemoved);
    }

    private void HandleModuleInserted(SelectEnterEventArgs arg)
    {
        _moduleID = arg.interactableObject.transform.GetComponent<Module>().ModuleID;
        if (!char.IsWhiteSpace(_moduleID))
        {
            Debug.Log($"[{nameof(ConsoleSlot)}] Slot '{_slotID}' -> Module '{_moduleID}' placed!");
            _controller.InsertModule(_slotID, _moduleID);
        }
    }

    private void HandleModuleRemoved(SelectExitEventArgs arg)
    {
        Debug.Log($"[{nameof(ConsoleSlot)}] Slot '{_slotID}' -> Module '{_moduleID}' removed!");
        _controller.ResetSlots();
    }
}
