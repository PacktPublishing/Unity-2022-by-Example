using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ConsoleController : MonoBehaviour
{
    [SerializeField] private string _consoleCode = "CBA";
    [SerializeField] private TMP_Text _consoleScreen;

    public UnityAction OnSuccess;
    public UnityAction OnFailure;

    private const char STARTING_SLOT = 'A';
    private const char MSG_INDICATOR = '~';
    private const string MSG_SOLVED = "ENERGIZED";

    private int _nextSlotIndex = 0;

    internal void InsertModule(char slotID, char moduleID)
    {
        var expectedSlotIndex = slotID - STARTING_SLOT;

        if (expectedSlotIndex != _nextSlotIndex)
        {
            Debug.Log("Incorrect slot. Please follow the order!");
            OnFailure?.Invoke();
            ResetSlots();
            return;
        }

        if (_consoleCode[_nextSlotIndex] != moduleID)
        {
            Debug.Log("Incorrect module placement. Please start over.");
            OnFailure?.Invoke();
            ResetSlots();
            return;
        }

        _nextSlotIndex++;

        if (_nextSlotIndex == _consoleCode.Length)
        {
            ConsoleEnergized();
            return;
        }

        _consoleScreen.SetText(new string(MSG_INDICATOR, _nextSlotIndex));
    }

    internal void ResetSlots() => _nextSlotIndex = 0;

    [ContextMenu("Trigger Console Energized")]
    public void ConsoleEnergized()
    {
        Debug.Log("Console energized!");

        _consoleScreen.SetText(MSG_SOLVED);

        EventSystem.Instance.TriggerEvent(EventConstants.OnConsoleEnergized, true);
        OnSuccess?.Invoke();
    }
}