using UnityEngine;

public class DebugConsoleLogger : MonoBehaviour
{
    public enum LogType
    {
        Normal,
        Warning,
        Error
    }

    [SerializeField]
    private LogType _logType = LogType.Normal;

    public void LogMessage(string message)
    {
        switch (_logType)
        {
            case LogType.Normal:
                Debug.Log(message);
                break;

            case LogType.Warning:
                Debug.LogWarning(message);
                break;

            case LogType.Error:
                Debug.LogError(message);
                break;
        }
    }
}