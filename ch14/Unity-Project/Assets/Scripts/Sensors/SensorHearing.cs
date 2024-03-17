using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorHearing
{
    public event UnityAction<AudioSource> OnAudioDetected;

    private readonly MonoBehaviour _context;
    private readonly float _hearingRange;
    private readonly float _updateFrequency = 2f;
    private readonly List<AudioSource> _audioSources = new();

    public SensorHearing(MonoBehaviour context,
        float hearingRange, float updateFrequency)
    {
        _context = context;
        _hearingRange = hearingRange;
        _updateFrequency = updateFrequency;

        _context.StartCoroutine(PeriodicallyUpdateAudioSources());

        IEnumerator PeriodicallyUpdateAudioSources()
        {
            // Cache for optimization.
            var wait = new WaitForSeconds(_updateFrequency);

            while (true)
            {
                UpdateAudioSources();
                yield return wait;
            }
        }
    }

    private void UpdateAudioSources()
    {
        _audioSources.Clear();

        var allAudioSources = Object.FindObjectsOfType<AudioSource>();

        foreach (var source in allAudioSources)
        {
            // Only add Player audio sources - requires AudioSource component to be on the Player (if using
            if (source.gameObject.CompareTag(Tags.Player))
            {
                _audioSources.Add(source);
            }
        }
    }

    private void IsAudioDetected()
    {
        foreach (var audioSource in _audioSources)
        {
            if (audioSource.isPlaying && CanHearAudioSource(audioSource))
            {
                OnAudioDetected?.Invoke(audioSource);
            }
        }
    }

    private bool CanHearAudioSource(AudioSource audioSource)
    {
        var distanceToSource = Vector3.Distance(
            _context.transform.position, audioSource.transform.position);

        var adjustedHearingRange = _hearingRange * audioSource.volume;

        return distanceToSource <= adjustedHearingRange;
    }

    public void Tick() => IsAudioDetected();
}