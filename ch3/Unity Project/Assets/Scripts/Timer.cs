using UnityEngine;
using UnityEngine.Events;

// Added ch3 - Update Pickup Count
public class Timer : MonoBehaviour
{
    public event UnityAction<int> OnTimeUpdate;
    public event UnityAction OnTimeExpired;

    private int _timeSeconds, _timeCurrent;

    public void StartTimer(int time)
    {
        _timeSeconds = time;
        _timeCurrent = 0;
        InvokeRepeating(nameof(UpdateTimer), 1f, 1f);
    }

    public void StopTimer() => CancelInvoke();

    private void UpdateTimer()
    {
        _timeCurrent++;
        OnTimeUpdate?.Invoke(_timeCurrent);

        if (_timeCurrent >= _timeSeconds)
        {
            StopTimer();
            OnTimeExpired?.Invoke();
        }
    }
}
